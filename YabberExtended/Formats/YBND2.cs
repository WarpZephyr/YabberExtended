using SoulsFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using YabberExtended.Extensions.Xml;
using static YabberExtended.YabberUtil;

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

            var pathCounts = new Dictionary<string, int>();
            xw.WriteStartElement("files");
            for (int i = 0; i < bnd.Files.Count; i++)
            {
                var file = bnd.Files[i];
                string strID = file.ID.ToString();

                string suffix = string.Empty;
                xw.WriteStartElement("file");
                xw.WriteAttributeString("id", strID);
                if (bnd.FilePathMode != BND2.FilePathModeEnum.Nameless && (bnd.FileInfoFlags & BND2.FileInfoFlagsEnum.NameOffset) != 0 && !string.IsNullOrEmpty(file.Name))
                {
                    xw.WriteElementString("name", file.Name);
                    if (pathCounts.TryGetValue(file.Name, out int value))
                    {
                        pathCounts[file.Name] = ++value;
                        suffix = $" ({value})";
                        xw.WriteElementString("suffix", suffix);
                    }
                    else
                    {
                        pathCounts[file.Name] = 1;
                    }
                }

                xw.WriteEndElement();

                string name = file.Name;
                if (string.IsNullOrEmpty(name))
                {
                    name = strID;
                }

                if (bnd.FilePathMode == BND2.FilePathModeEnum.BaseDirectory)
                    name = Path.Combine(RemoveRootFromPath(bnd.BaseDirectory), RemoveLeadingSlashes(name));

                string outPath = Path.Combine(targetDir, RemoveRootFromPath(name));
                string? outDir = Path.GetDirectoryName(outPath);
                if (string.IsNullOrEmpty(outDir))
                {
                    throw new Exception($"Failed to get output folder name for output path: {outPath}");
                }

                if (!string.IsNullOrEmpty(suffix))
                {
                    string outName = Path.GetFileNameWithoutExtension(outPath);
                    string outExtension = Path.GetExtension(outPath);
                    outPath = $"{Path.Combine(outDir, outName)}{suffix}{outExtension}";
                }

                Directory.CreateDirectory(outDir);
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

            string binderName = xml.ReadStringOrThrowIfWhiteSpace("bnd2/binder_name");
            bnd.HeaderInfoFlags = (BND2.HeaderInfoFlagsEnum)xml.ReadByte("bnd2/header_info_flags");
            bnd.FileInfoFlags = (BND2.FileInfoFlagsEnum)xml.ReadByte("bnd2/file_info_flags");
            bnd.Unk06 = xml.ReadByteOrDefault("bnd2/unk06");
            bnd.Unk07 = xml.ReadByteOrDefault("bnd2/unk07");
            bnd.FileVersion = xml.ReadInt32("bnd2/file_version");
            bnd.AlignmentSize = xml.ReadUInt16("bnd2/alignment_size");
            bnd.Unk1B = xml.ReadByteOrDefault("bnd2/unk1B");

            var filePathMode = xml.ReadEnum<BND2.FilePathModeEnum>("bnd2/file_path_mode");
            bnd.FilePathMode = filePathMode;
            if (bnd.FilePathMode == BND2.FilePathModeEnum.BaseDirectory)
            {
                bnd.BaseDirectory = xml.ReadStringOrDefault("bnd2/base_directory", string.Empty);
            }
            else
            {
                bnd.BaseDirectory = string.Empty;
            }

            var filesNode = xml.SelectNodes("bnd2/files/file");
            if (filesNode != null)
            {
                foreach (XmlNode fileNode in filesNode)
                {
                    var attributes = fileNode.GetAttributesOrThrow();
                    int id = attributes.ReadInt32("id");
                    string name = fileNode.ReadStringOrDefault("name", string.Empty);
                    string suffix = fileNode.ReadStringOrDefault("suffix", string.Empty);

                    string inPath = string.Empty;
                    if ((bnd.FileInfoFlags & BND2.FileInfoFlagsEnum.NameOffset) != 0)
                    {
                        switch (filePathMode)
                        {
                            case BND2.FilePathModeEnum.Nameless:
                                inPath = Path.Combine(sourceDir, id.ToString());
                                break;
                            case BND2.FilePathModeEnum.FileName:
                                inPath = Path.Combine(sourceDir, name);
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

                        if (!string.IsNullOrEmpty(suffix))
                        {
                            string? inDir = Path.GetDirectoryName(inPath);
                            if (string.IsNullOrEmpty(inDir))
                            {
                                throw new Exception($"Failed to get input folder name for input path: {inPath}");
                            }

                            string inName = Path.GetFileNameWithoutExtension(inPath);
                            string inExtension = Path.GetExtension(inPath);
                            inPath = $"{Path.Combine(inDir, inName)}{suffix}{inExtension}";
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
            BackupFile(outPath);
            bnd.Write(outPath);
        }
    }
}
