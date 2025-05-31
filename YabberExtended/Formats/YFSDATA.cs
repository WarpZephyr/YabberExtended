using SoulsFormats;
using System;
using System.IO;
using System.Xml;
using YabberExtended.Extensions;
using YabberExtended.Extensions.Value;
using YabberExtended.Extensions.Xml;
using static YabberExtended.YabberUtil;

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
            string fsdataName = xml.ReadStringOrThrowIfWhiteSpace("fsdata/fsdata_name");
            int entryCount = xml.ReadInt32("fsdata/entry_count");
            bool compressed = xml.ReadBoolean("fsdata/compressed");
            var data = new FSDATA(entryCount, compressed);

            var fileNodes = xml.SelectNodes("fsdata/files/file");
            if (fileNodes != null)
            {
                foreach (XmlNode fileNode in fileNodes)
                {
                    var file = new FSDATA.File();
                    string strID = fileNode.GetNodeOrThrow("id").InnerText;
                    int id = strID.ToInt32("id");

                    string inPath = fileNode.GetFilePathNameOrUseID(sourceDir, strID);
                    FriendlyException.ThrowIfNotFile(inPath);

                    file.ID = id;
                    file.Bytes = File.ReadAllBytes(inPath);
                    data.Files.Add(file);
                }
            }

            string outPath = Path.Combine(targetDir, fsdataName);
            BackupFile(outPath);
            data.Write(outPath);
        }
    }
}
