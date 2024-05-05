using System;
using System.IO;
using System.Xml;
using SoulsFormats.Kuon;
using YabberExtended.Extensions.Xml;
using static YabberExtended.YabberUtil;

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

            string binderName = xml.ReadStringOrThrowIfWhiteSpace("bnd_kuon/binder_name");
            bnd.FileVersion = xml.ReadInt32("bnd_kuon/file_version");
            bool hasEntrySizeField = xml.ReadBooleanOrDefault("bnd_kuon/has_entry_size_field");

            var fileNodes = xml.SelectNodes("bnd_kuon/files/file");
            if (fileNodes != null)
            {
                foreach (XmlNode fileNode in fileNodes)
                {
                    int id = xml.ReadInt32("id");
                    string name = fileNode.ReadStringOrDefault("name", id.ToString());
                    string inPath = Path.Combine(sourceDir, name);

                    if (!File.Exists(inPath))
                        throw new FriendlyException($"File not found: {inPath}");

                    inPath = CorrectDirectorySeparator(inPath);
                    bnd.Files.Add(new BND.File(id, name, File.ReadAllBytes(inPath)));
                }
            }

            string outPath = Path.Combine(targetDir, binderName);
            BackupFile(outPath);

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
