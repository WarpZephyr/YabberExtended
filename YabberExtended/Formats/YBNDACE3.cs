using System;
using System.IO;
using System.Xml;
using SoulsFormats.ACE3;
using static YabberExtended.YBUtil;

namespace YabberExtended
{
    static class YBNDACE3
    {
        public static void Unpack(this BND bnd, string sourceName, string targetDir, IProgress<float> progress)
        {
            Directory.CreateDirectory(targetDir);
            var xws = new XmlWriterSettings();
            xws.Indent = true;
            var xw = XmlWriter.Create(Path.Combine(targetDir, "_yabber-bnd_ace3.xml"), xws);
            xw.WriteStartElement("bnd_ace3");
            xw.WriteElementString("binder_name", sourceName);
            xw.WriteElementString("lite", bnd.Lite.ToString());
            xw.WriteElementString("flag1", bnd.Flag1.ToString());
            xw.WriteElementString("flag2", bnd.Flag2.ToString());

            xw.WriteStartElement("files");
            for (int i = 0; i < bnd.Files.Count; i++)
            {
                xw.WriteStartElement("file");
                var file = bnd.Files[i];
                string strID = file.ID.ToString();

                xw.WriteElementString("id", strID);
                xw.WriteEndElement();

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
            xml.Load(Path.Combine(sourceDir, "_yabber-bnd_ace3.xml"));

            string binderName = FieldToString(xml.SelectSingleNode("bnd_ace3/binder_name")?.InnerText, "binder_name");
            bnd.Lite = FieldToBool(xml.SelectSingleNode("bnd_ace3/lite")?.InnerText, "lite");
            bnd.Flag1 = FieldToByte(xml.SelectSingleNode("bnd_ace3/flag1")?.InnerText, "flag1");
            bnd.Flag2 = FieldToByte(xml.SelectSingleNode("bnd_ace3/flag2")?.InnerText, "flag2");

            var filesNode = xml.SelectSingleNode("bnd_ace3/files");
            if (filesNode != null)
            {
                foreach (XmlNode fileNode in filesNode.SelectNodes("file"))
                {
                    int id = FieldToInt32(fileNode.SelectSingleNode("id")?.InnerText, "id");

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
