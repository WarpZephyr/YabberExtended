using System;
using System.IO;
using System.Xml;
using SoulsFormats.AC3SL;
using YabberExtended.Extensions;
using YabberExtended.Extensions.Value;
using YabberExtended.Extensions.Xml;
using static YabberExtended.YabberUtil;

namespace YabberExtended
{
    static class YBNDAC3SL
    {
        public static void Unpack(this BND bnd, string sourceName, string targetDir, IProgress<float> progress)
        {
            Directory.CreateDirectory(targetDir);
            var xws = new XmlWriterSettings();
            xws.Indent = true;
            var xw = XmlWriter.Create(Path.Combine(targetDir, "_yabber-bnd_ac3sl.xml"), xws);
            xw.WriteStartElement("bnd_ac3sl");
            xw.WriteElementString("binder_name", sourceName);
            xw.WriteElementString("file_version", bnd.FileVersion);
            xw.WriteElementString("alignment_size", bnd.AlignmentSize.ToString());
            xw.WriteElementString("unk1E", bnd.Unk1E.ToString());

            xw.WriteStartElement("files");
            for (int i = 0; i < bnd.Files.Count; i++)
            {
                var file = bnd.Files[i];
                string strID = file.ID.ToString();

                xw.WriteElementString("file", strID);

                string outPath = Path.Combine(targetDir, strID);
                Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                File.WriteAllBytes(outPath, file.Bytes);
                progress.Report((float)i / bnd.Files.Count);
            }

            xw.WriteEndElement();
            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string sourceDir, string targetDir)
        {
            var bnd = new BND();
            var xml = new XmlDocument();
            xml.Load(Path.Combine(sourceDir, "_yabber-bnd_ac3sl.xml"));

            string binderName = xml.ReadStringOrThrowIfWhiteSpace("bnd_ac3sl/binder_name");
            bnd.FileVersion = xml.ReadStringOrDefault("bnd_ac3sl/file_version", "LTL");
            bnd.AlignmentSize = xml.ReadInt16("bnd_ac3sl/alignment_size");
            bnd.Unk1E = xml.ReadInt16("bnd_ac3sl/unk1E");

            var fileNodes = xml.SelectNodes("bnd_ac3sl/files/file");
            if (fileNodes != null)
            {
                foreach (XmlNode fileNode in fileNodes)
                {
                    string id = fileNode.GetXmlValueOrContents("id");
                    string inPath = fileNode.GetFilePathNameOrUseID(sourceDir, id);
                    FriendlyException.ThrowIfNotFile(inPath);
                    bnd.Files.Add(new BND.File(id.ToInt32(), File.ReadAllBytes(inPath)));
                }
            }

            string outPath = Path.Combine(targetDir, binderName);
            BackupFile(outPath);
            bnd.Write(outPath);
        }
    }
}
