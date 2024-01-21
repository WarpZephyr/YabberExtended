using System;
using System.IO;
using System.Xml;
using SoulsFormats.AC3SL;
using static YabberExtended.YBUtil;

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

                xw.WriteElementString("id", strID);

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

            string binderName = FieldToString(xml.SelectSingleNode("bnd_ac3sl/binder_name")?.InnerText, "binder_name");
            bnd.FileVersion = FieldToString(xml.SelectSingleNode("bnd_ac3sl/file_version")?.InnerText, "file_version");
            bnd.AlignmentSize = FieldToInt16(xml.SelectSingleNode("bnd_ac3sl/alignment_size")?.InnerText, "alignment_size");
            bnd.Unk1E = FieldToInt16(xml.SelectSingleNode("bnd_ac3sl/unk1E")?.InnerText, "unk1E");

            var filesNode = xml.SelectSingleNode("bnd_ac3sl/files");
            if (filesNode != null)
            {
                foreach (XmlNode fileNode in filesNode.SelectNodes("id"))
                {
                    int id = FieldToInt32(fileNode?.InnerText, "id");

                    string inPath = Path.Combine(sourceDir, id.ToString());

                    if (!File.Exists(inPath))
                        throw new FriendlyException($"File not found: {inPath}");

                    inPath = CorrectDirectorySeparator(inPath);
                    bnd.Files.Add(new BND.File(id, File.ReadAllBytes(inPath)));
                }
            }

            string outPath = Path.Combine(targetDir, binderName);
            Backup(outPath);

            bnd.Write(outPath);
        }
    }
}
