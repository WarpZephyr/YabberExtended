using SoulsFormats;
using System;
using System.IO;
using System.Xml;

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
    }
}
