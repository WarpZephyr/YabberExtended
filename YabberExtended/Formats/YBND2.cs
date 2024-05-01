using System;
using System.IO;
using System.Xml;
using SoulsFormats;
using static YabberExtended.YBUtil;

namespace YabberExtended
{
    static class YBND2
    {
        public static void Unpack(this BND2Reader bnd, string sourceName, string targetDir, IProgress<float> progress)
        {
            Directory.CreateDirectory(targetDir);
            var xws = new XmlWriterSettings();
            xws.Indent = true;
            var xw = XmlWriter.Create(Path.Combine(targetDir, "_yabber-bnd2.xml"), xws);
            xw.WriteStartElement("bnd2");
            xw.WriteElementString("binder_name", sourceName);
            xw.WriteElementString("header_info_flags", ((byte)bnd.HeaderInfoFlags).ToString());
            xw.WriteElementString("file_info_flags", ((byte)bnd.FileInfoFlags).ToString());
            xw.WriteElementString("unk06", bnd.Unk06.ToString());
            xw.WriteElementString("unk07", bnd.Unk07.ToString());
            xw.WriteElementString("file_version", bnd.FileVersion.ToString());
            xw.WriteElementString("alignment_size", bnd.AlignmentSize.ToString());
            xw.WriteElementString("file_path_mode", bnd.FilePathMode.ToString());
            xw.WriteElementString("unk1B", bnd.Unk1B.ToString());
            if (bnd.FilePathMode == BND2.FilePathModeEnum.BaseDirectory)
                xw.WriteElementString("base_directory", bnd.BaseDirectory);

            xw.WriteStartElement("files");
            for (int i = 0; i < bnd.Files.Count; i++)
            {
                var file = bnd.Files[i];
                string strID = file.ID.ToString();

                xw.WriteStartElement("file");
                xw.WriteAttributeString("id", strID);
                if (bnd.FilePathMode != BND2.FilePathModeEnum.Nameless && (bnd.FileInfoFlags & BND2.FileInfoFlagsEnum.NameOffset) != 0 && !string.IsNullOrEmpty(file.Name))
                    xw.WriteString(file.Name);
                xw.WriteEndElement();

                string name = file.Name;
                if (string.IsNullOrEmpty(name))
                {
                    name = strID;
                }

                if (bnd.FilePathMode == BND2.FilePathModeEnum.BaseDirectory)
                    name = Path.Combine(RemoveRootFromPath(bnd.BaseDirectory), RemoveLeadingSlashes(name));

                string outPath = Path.Combine(targetDir, RemoveRootFromPath(name));
                Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                File.WriteAllBytes(outPath, bnd.ReadFile(file));
                progress.Report((float)i / bnd.Files.Count);
            }

            xw.WriteEndElement();
            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string sourceDir, string targetDir)
        {
            var bnd = new BND2();
            var xml = new XmlDocument();
            xml.Load(Path.Combine(sourceDir, "_yabber-bnd2.xml"));

            string binderName = FieldToString(xml.SelectSingleNode("bnd2/binder_name")?.InnerText, "binder_name");
            bnd.HeaderInfoFlags = (BND2.HeaderInfoFlagsEnum)FieldToByte(xml.SelectSingleNode("bnd2/header_info_flags")?.InnerText, "header_info_flags");
            bnd.FileInfoFlags = (BND2.FileInfoFlagsEnum)FieldToByte(xml.SelectSingleNode("bnd2/file_info_flags")?.InnerText, "file_info_flags");
            bnd.Unk06 = FieldToByte(xml.SelectSingleNode("bnd2/unk06")?.InnerText, "unk06");
            bnd.Unk07 = FieldToByte(xml.SelectSingleNode("bnd2/unk07")?.InnerText, "unk07");
            bnd.FileVersion = FieldToInt32(xml.SelectSingleNode("bnd2/file_version")?.InnerText, "file_version");
            bnd.AlignmentSize = FieldToUInt16(xml.SelectSingleNode("bnd2/alignment_size")?.InnerText, "alignment_size");
            bnd.Unk1B = FieldToByte(xml.SelectSingleNode("bnd2/unk1B")?.InnerText, "unk1B", 0);

            string strFilePathMode = xml.SelectSingleNode("bnd2/file_path_mode")?.InnerText;
            BND2.FilePathModeEnum filePathMode;
            switch (strFilePathMode)
            {
                case "0":
                case "Nameless":
                    filePathMode = BND2.FilePathModeEnum.Nameless;
                    break;
                case "1":
                case "FileName":
                    filePathMode = BND2.FilePathModeEnum.FileName;
                    break;
                case "2":
                case "FullPath":
                    filePathMode = BND2.FilePathModeEnum.FullPath;
                    break;
                case "3":
                case "BaseDirectory":
                    filePathMode = BND2.FilePathModeEnum.BaseDirectory;
                    break;
                default:
                    throw new FriendlyException($"{strFilePathMode} is not a valid file path mode.");
            }
            bnd.FilePathMode = filePathMode;

            if (bnd.FilePathMode == BND2.FilePathModeEnum.BaseDirectory)
            {
                bnd.BaseDirectory = xml.SelectSingleNode("bnd2/base_directory")?.InnerText ?? string.Empty;
            }

            var filesNode = xml.SelectSingleNode("bnd2/files");
            if (filesNode != null)
            {
                foreach (XmlNode fileNode in filesNode.SelectNodes("file"))
                {
                    int id = FieldToInt32(fileNode.Attributes["id"]?.InnerText, "id");
                    string name = fileNode?.InnerText ?? string.Empty;

                    string inPath = string.Empty;
                    if ((bnd.FileInfoFlags & BND2.FileInfoFlagsEnum.NameOffset) != 0)
                    {
                        switch (filePathMode)
                        {
                            case BND2.FilePathModeEnum.Nameless:
                                inPath = Path.Combine(sourceDir, id.ToString());
                                break;
                            case BND2.FilePathModeEnum.FileName:
                                inPath = Path.Combine(sourceDir, Path.GetDirectoryName(name), $"{Path.GetFileNameWithoutExtension(name)}{Path.GetExtension(name)}");
                                break;
                            case BND2.FilePathModeEnum.FullPath:
                                inPath = Path.Combine(sourceDir, RemoveRootFromPath(name));
                                break;
                            case BND2.FilePathModeEnum.BaseDirectory:
                                inPath = Path.Combine(sourceDir, RemoveRootFromPath(bnd.BaseDirectory), RemoveLeadingSlashes(name));
                                break;
                            default:
                                throw new FriendlyException($"{filePathMode} is not a valid file path mode.");
                        }
                    }
                    else
                    {
                        inPath = Path.Combine(sourceDir, id.ToString());
                    }
                    
                    if (!File.Exists(inPath))
                        throw new FriendlyException($"File not found: {inPath}");

                    inPath = CorrectDirectorySeparator(inPath);
                    bnd.Files.Add(new BND2.File(id, name, File.ReadAllBytes(inPath)));
                }
            }

            string outPath = Path.Combine(targetDir, binderName);
            Backup(outPath);
            bnd.Write(outPath);
        }
    }
}
