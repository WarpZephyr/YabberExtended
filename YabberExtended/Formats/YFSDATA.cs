using SoulsFormats;
using System;
using System.IO;
using System.Xml;
using static YabberExtended.YBUtil;

namespace YabberExtended
{
    static class YFSDATA
    {
        public static void Unpack(this FSDATA data, string sourceName, string targetDir, IProgress<float> progress)
        {
            Directory.CreateDirectory(targetDir);
            var xws = new XmlWriterSettings();
            xws.Indent = true;
            var xw = XmlWriter.Create(Path.Combine(targetDir, "_yabber-fsdata.xml"), xws);
            xw.WriteStartElement("fsdata");
            xw.WriteElementString("fsdata_name", sourceName);
            xw.WriteElementString("entry_count", data.EntryCount.ToString());
            xw.WriteElementString("compressed", data.Compressed.ToString());

            xw.WriteStartElement("files");
            int count = data.Files.Count;
            for (int i = 0; i < count; i++)
            {
                var file = data.Files[i];
                string strID = file.ID.ToString();
                xw.WriteStartElement("file");
                xw.WriteElementString("id", strID);
                xw.WriteEndElement();

                string outPath = Path.Combine(targetDir, strID);
                File.WriteAllBytes(outPath, file.Bytes);
                progress.Report((float)i / count);
            }
            xw.WriteEndElement();

            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string sourceDir, string targetDir)
        {
            var xml = new XmlDocument();
            xml.Load(Path.Combine(sourceDir, "_yabber-fsdata.xml"));
            string fsdataName = FieldToString(xml.SelectSingleNode("fsdata/fsdata_name")?.InnerText, "fsdata_name");
            int entryCount = FieldToInt32(xml.SelectSingleNode("fsdata/entry_count")?.InnerText, "entry_count");
            bool compressed = FieldToBool(xml.SelectSingleNode("fsdata/compressed")?.InnerText, "compressed");
            var data = new FSDATA(entryCount, compressed);

            var filesNode = xml.SelectSingleNode("fsdata/files");
            if (filesNode != null)
            {
                foreach (XmlNode fileNode in filesNode.SelectNodes("file"))
                {
                    var file = new FSDATA.File();
                    string strID = fileNode.SelectSingleNode("id")?.InnerText;
                    int id = FieldToInt32(strID, "id");
                    
                    string inPath = Path.Combine(sourceDir, strID);
                    if (!File.Exists(inPath))
                    {
                        throw new FriendlyException($"File not found: {inPath}");
                    }

                    file.ID = id;
                    file.Bytes = File.ReadAllBytes(inPath);
                    data.Files.Add(file);
                }
            }

            string outPath = Path.Combine(targetDir, fsdataName);
            Backup(outPath);
            data.Write(outPath);
        }
    }
}
