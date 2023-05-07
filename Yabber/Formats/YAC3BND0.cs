using SoulsFormats.AC3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Yabber.Formats
{
    static class YAC3BND0
    {
        public static void Unpack(this BND0 bnd, string sourceName, string targetDir, IProgress<float> progress)
        {
            Directory.CreateDirectory(targetDir);
            var xws = new XmlWriterSettings();
            xws.Indent = true;
            var xw = XmlWriter.Create($"{targetDir}\\_yabber-ac3bnd0.xml", xws);
            xw.WriteStartElement("AC3-BND0");

            xw.WriteElementString("BNDName", sourceName);
            xw.WriteElementString("Version", bnd.Version.ToString());
            xw.WriteElementString("Unk1B", bnd.Unk1B.ToString());
            xw.WriteElementString("Alignment", bnd.Alignment.ToString());
            if (bnd.Version == NameVersion.NamesOffset)
                xw.WriteElementString("Path", bnd.Path);
            xw.WriteElementString("Unk08", bnd.Unk08.ToString());
            xw.WriteStartElement("Files");

            for (int i = 0; i < bnd.Files.Count; i++)
            {
                xw.WriteStartElement("File");
                var file = bnd.Files[i];
                if (bnd.Version != NameVersion.Nameless)
                {
                    if(bnd.Version == NameVersion.NamesOffset)
                        xw.WriteElementString("FileName", file.Name.Substring(bnd.Path.Substring(2).Length - 1));
                    else
                        xw.WriteElementString("FileName", file.Name);
                }
                xw.WriteElementString("ID", file.ID.ToString());
                xw.WriteElementString("Size", file.Bytes.Length.ToString());
                xw.WriteEndElement();

                string name = file.Name;
                if (name == null) name = $"{file.ID}";
                string outPath = $"{targetDir}\\{name}";
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
            var bnd = new BND0();
            var xml = new XmlDocument();
            xml.Load($"{sourceDir}\\_yabber-ac3bnd0.xml");

            if (xml.SelectSingleNode("AC3-BND0/BNDName") == null)
                throw new FriendlyException("Missing BNDName tag.");

            string filename = xml.SelectSingleNode("AC3-BND0/BNDName").InnerText;
            string strVersion = xml.SelectSingleNode("AC3-BND0/Version").InnerText;
            string strUnk1B = xml.SelectSingleNode("AC3-BND0/Unk1B").InnerText;
            string strAlignment = xml.SelectSingleNode("AC3-BND0/Alignment").InnerText;
            string strUnk08 = xml.SelectSingleNode("AC3-BND0/Unk08").InnerText;


            if (filename == null)
                throw new FriendlyException("BNDName tag is missing, do not know what to name repacked AC3 BND0.");
            if (filename.Length == 0)
                throw new FriendlyException("BNDName tag cannot be empty.");
            if (strVersion == null)
                throw new FriendlyException($"Version tag is missing."); ;
            NameVersion version;
            switch (strVersion)
            {
                case "Nameless":
                    version = NameVersion.Nameless;
                    break;
                case "NamesNoOffset":
                    version = NameVersion.NamesNoOffset;
                    break;
                case "Paths":
                    version = NameVersion.Paths;
                    break;
                case "NamesOffset":
                    version = NameVersion.NamesOffset;
                    break;
                default:
                    throw new FriendlyException($"{strVersion} is not a AC3 BND0 version and is invalid.");
            }
            if (strAlignment == null)
                throw new FriendlyException("Alignment tag missing.");
            try
            {
                Convert.ToUInt16(strAlignment);
            }
            catch
            {
                throw new FriendlyException("Alignment value invalid, it must be an unsigned, 16 bit integer.");
            }
            if (strUnk08 == null)
                throw new FriendlyException("Unk08 tag missing, set it to either 202 or 211.");
            try
            {
                Convert.ToInt32(strUnk08);
            }
            catch
            {
                throw new FriendlyException("Unk08 value invalid, it must be a signed, 32 bit integer.");
            }
            if (strUnk1B == null)
                throw new FriendlyException("Unk1B tag missing, set it to either a 0 or a 1.");
            try
            {
                Convert.ToByte(strUnk1B);
            }
            catch
            {
                throw new FriendlyException("Unk08 value invalid, it must be an unsigned, 8 bit integer.");
            }

            bnd.Version = version;
            bnd.Unk1B = byte.Parse(strUnk1B);
            bnd.Alignment = ushort.Parse(strAlignment);
            if (bnd.Version == NameVersion.NamesOffset)
            {
                string namePath = xml.SelectSingleNode("AC3-BND0/Path").InnerText;
                if (namePath == null)
                    throw new FriendlyException("Version is NamesOffset but the Path tag with the path to files is missing.");
                if (namePath.Length == 0)
                    throw new FriendlyException("Path must not be empty");
                bnd.Path = namePath; 
            }
            bnd.Unk08 = int.Parse(strUnk08);

            var filesNode = xml.SelectSingleNode("AC3-BND0/Files");
            if (filesNode == null)
                throw new FriendlyException("There must be files to repack, Files tag is missing.");

            bnd.Files = new List<BND0.File>();
            foreach (XmlNode fileNode in filesNode.SelectNodes("File"))
            {
                string strID = fileNode.SelectSingleNode("ID")?.InnerText ?? "Null";
                string name = fileNode.SelectSingleNode("FileName")?.InnerText ?? "Null";
                string size = fileNode.SelectSingleNode("Size")?.InnerText ?? "Null";

                if (name == "Null" && version != NameVersion.Nameless)
                    throw new FriendlyException("File node missing Name tag for BND0 with names.");

                if (strID == "Null")
                    throw new FriendlyException("File node missing ID tag.");

                if (size == "Null")
                    Console.WriteLine("File node missing Size tag, either removed after unpack or AC3 BND0 unpacked wrong.");

                if (!int.TryParse(strID, out int id))
                    throw new FriendlyException($"Could not parse file ID: {strID}\nID must be a 32-bit signed integer.");

                string inPath = string.Empty;
                if (version == NameVersion.Nameless)
                    inPath = $"{sourceDir}\\{Path.GetDirectoryName(id.ToString())}\\{Path.GetFileNameWithoutExtension(id.ToString())}{Path.GetExtension(id.ToString())}";
                else if (version == NameVersion.NamesNoOffset)
                    inPath = $"{sourceDir}\\{Path.GetDirectoryName(name)}\\{Path.GetFileNameWithoutExtension(name)}{Path.GetExtension(name)}";
                else if (version == NameVersion.Paths)
                    inPath = $"{sourceDir}\\{name}";
                else if (version == NameVersion.NamesOffset)
                    inPath = $"{sourceDir}\\{bnd.Path.Substring(2)}{name}";
                if (version == NameVersion.Paths)
                    name = $"K:\\{name}";
                if (!File.Exists(inPath))
                    throw new FriendlyException($"File not found: {inPath}");
                
                byte[] bytes = File.ReadAllBytes(inPath);
                bnd.Files.Add(new BND0.File(id, name, bytes));
            }

            string outPath = $"{targetDir}\\{filename}";
            YBUtil.Backup(outPath);
            bnd.Write(outPath);
        }
    }
}
