using System;
using System.IO;
using System.Xml;
using SoulsFormats.Kuon;
using static YabberExtended.YBUtil;

namespace YabberExtended
{
    static class YBNDKUON
    {
        public static void Unpack(this BND bnd, string sourceName, string targetDir, IProgress<float> progress)
        {
            Directory.CreateDirectory(targetDir);
            var xws = new XmlWriterSettings();
            xws.Indent = true;
            var xw = XmlWriter.Create(Path.Combine(targetDir, "_yabber-bnd_kuon.xml"), xws);
            xw.WriteStartElement("bnd_kuon");
            xw.WriteElementString("binder_name", sourceName);
            xw.WriteElementString("file_version", bnd.FileVersion.ToString());
            xw.WriteElementString("has_entry_size_field", false.ToString());

            xw.WriteStartElement("files");
            for (int i = 0; i < bnd.Files.Count; i++)
            {
                xw.WriteStartElement("file");
                var file = bnd.Files[i];
                string strID = file.ID.ToString();

                xw.WriteElementString("name", file.Name);
                xw.WriteElementString("id", strID);
                xw.WriteEndElement();

                string name = file.Name ?? strID;
                string outPath = Path.Combine(targetDir, RemoveRootFromPath(name));
                Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                File.WriteAllBytes(outPath, file.Bytes);
                progress.Report((float)i / bnd.Files.Count);
            }

            xw.WriteEndElement();
            xw.WriteEndElement();
            xw.Close();
        }

        public static void Unpack(this DVDBND bnd, string sourceName, string targetDir, IProgress<float> progress)
        {
            Directory.CreateDirectory(targetDir);
            var xws = new XmlWriterSettings();
            xws.Indent = true;
            var xw = XmlWriter.Create(Path.Combine(targetDir, "_yabber-bnd_kuon.xml"), xws);
            xw.WriteStartElement("bnd_kuon");
            xw.WriteElementString("binder_name", sourceName);
            xw.WriteElementString("file_version", bnd.FileVersion.ToString());
            xw.WriteElementString("has_entry_size_field", true.ToString());

            xw.WriteStartElement("files");
            for (int i = 0; i < bnd.Files.Count; i++)
            {
                xw.WriteStartElement("file");
                var file = bnd.Files[i];
                xw.WriteElementString("name", file.Name);
                xw.WriteElementString("id", file.ID.ToString());
                xw.WriteEndElement();

                string name = file.Name;
                if (string.IsNullOrEmpty(name))
                {
                    name = file.ID.ToString();
                }

                string outPath = Path.Combine(targetDir, RemoveRootFromPath(name));
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
            xml.Load(Path.Combine(sourceDir, "_yabber-bnd_kuon.xml"));

            string binderName = FieldToString(xml.SelectSingleNode("bnd_kuon/binder_name")?.InnerText, "binder_name");
            bnd.FileVersion = FieldToInt32(xml.SelectSingleNode("bnd_kuon/file_version")?.InnerText, "file_version");
            bool hasEntrySizeField = FieldToBool(xml.SelectSingleNode("bnd_kuon/has_entry_size_field")?.InnerText, "has_entry_size_field");

            var filesNode = xml.SelectSingleNode("bnd_kuon/files");
            if (filesNode != null)
            {
                foreach (XmlNode fileNode in filesNode.SelectNodes("file"))
                {
                    int id = FieldToInt32(fileNode.SelectSingleNode("id")?.InnerText, "id");
                    string name = fileNode.SelectSingleNode("name")?.InnerText ?? id.ToString();

                    string inPath = Path.Combine(sourceDir, RemoveRootFromPath(name));

                    if (!File.Exists(inPath))
                        throw new FriendlyException($"File not found: {inPath}");

                    inPath = CorrectDirectorySeparator(inPath);
                    bnd.Files.Add(new BND.File(id, name, File.ReadAllBytes(inPath)));
                }
            }

            string outPath = Path.Combine(targetDir, binderName);
            Backup(outPath);

            if (hasEntrySizeField)
            {
                new DVDBND(bnd).Write(outPath);
            }
            else
            {
                bnd.Write(outPath);
            }
        }
    }
}
