using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SoulsFormats;
using YabberExtended.Extensions.Xml;
using static YabberExtended.YabberUtil;

namespace YabberExtended
{
    static class YZero3
    {
        public static void Unpack(this Zero3 z3, string sourceName, string targetDir)
        {
            Directory.CreateDirectory(targetDir);
            var xws = new XmlWriterSettings
            {
                Indent = true
            };

            var xw = XmlWriter.Create(Path.Combine(targetDir, "_yabber-zero3.xml"), xws);
            xw.WriteStartElement("zero3");
            xw.WriteElementString("zero3_name", sourceName);
            xw.WriteElementString("version", z3.Version.ToString());
            xw.WriteElementString("storeinheader", z3.StoreInHeader.ToString());

            if (z3.Version == Zero3.FormatVersion.ArmoredCore4)
            {
                xw.WriteElementString("offsetalign", $"0x{z3.OffsetAlign:X}");
                xw.WriteElementString("sizealign", $"0x{z3.SizeAlign:X}");
                xw.WriteElementString("maxfilesize", $"0x{z3.MaxFileSize:X}");
                xw.WriteElementString("iswrapped", z3.IsWrapped.ToString());
                xw.WriteElementString("wrapperversion", z3.WrapperVersion);
                xw.WriteElementString("wrappedname", z3.WrappedName);
            }

            xw.WriteStartElement("files");
            foreach (Zero3.File file in z3.Files)
            {
                xw.WriteStartElement("file");
                string name = file.Name.Replace('/', '\\');
                name = SplitBinderRootFromPath(name, out string root);
                if (root != string.Empty)
                {
                    xw.WriteElementString("root", root);
                }

                xw.WriteElementString("name", name);
                xw.WriteEndElement();

                string outPath = Path.Combine(targetDir, name);
                Directory.CreateDirectory(Path.GetDirectoryName(outPath) ?? throw new Exception($"Could not get folder name for: {outPath}"));
                File.WriteAllBytes(outPath, file.Bytes);
            }
            xw.WriteEndElement();

            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string sourceDir, string targetDir)
        {
            // Parse xml
            var xml = new XmlDocument();
            xml.Load(Path.Combine(sourceDir, "_yabber-zero3.xml"));
            string zero3Name = xml.ReadStringOrThrowIfWhiteSpace("zero3/zero3_name");
            var version = xml.ReadEnum<Zero3.FormatVersion>("zero3/version");
            bool storeInHeader = xml.ReadBooleanOrDefault("zero3/storeinheader");
            var z3 = new Zero3
            {
                Version = version,
                StoreInHeader = storeInHeader,
            };

            // Calculate backup wrapper name in case user has not specified it.
            // Add .000 extension if user has not specified it.
            string backupWrapperName;
            if (zero3Name.EndsWith(".000"))
            {
                backupWrapperName = zero3Name[..^4];
            }
            else
            {
                backupWrapperName = zero3Name;
                zero3Name = $"{zero3Name}.000";
            }

            // Parse version specific info
            if (version == Zero3.FormatVersion.ArmoredCore4)
            {
                z3.HeaderDataAlign = Zero3.DefaultHeaderDataAlignAC4;
                z3.OffsetAlign = xml.ReadInt32OrDefault("zero3/offsetalign");
                z3.SizeAlign = xml.ReadInt32OrDefault("zero3/sizealign");
                z3.MaxFileSize = xml.ReadInt32OrDefault("zero3/maxfilesize");
                z3.IsWrapped = xml.ReadBooleanOrDefault("zero3/iswrapped");
                z3.WrapperVersion = xml.ReadStringOrDefault("zero3/wrapperversion", Zero3.DefaultWrapperVersion);

                string wrappedName = xml.ReadStringOrDefault("zero3/wrappedname", backupWrapperName);
                if (string.IsNullOrWhiteSpace(wrappedName))
                {
                    throw new FriendlyException("zero3_name and wrappedname must not be empty or blank.");
                }

                z3.WrappedName = wrappedName;
            }
            else if (version == Zero3.FormatVersion.Murakumo)
            {
                z3.HeaderDataAlign = Zero3.DefaultHeaderDataAlignMurakumo;
                z3.OffsetAlign = Zero3.DefaultOffsetAlign;
                z3.SizeAlign = Zero3.DefaultSizeAlign;
                z3.MaxFileSize = Zero3.DefaultMaxFileSizeMurakumo;
                z3.IsWrapped = false;
                z3.WrapperVersion = string.Empty;
                z3.WrappedName = string.Empty;
            }
            else
            {
                throw new NotSupportedException($"Unknown {nameof(Zero3.FormatVersion)}: {version}");
            }

            // Parse files
            var fileNodes = xml.SelectNodes("zero3/files/file");
            if (fileNodes != null)
            {
                foreach (XmlNode fileNode in fileNodes)
                {
                    string rootName = fileNode.ReadStringOrDefault("root", string.Empty);
                    string fileName = fileNode.ReadString("name");
                    string finalName = $"{rootName}{fileName}";

                    string inPath = Path.Combine(sourceDir, fileName);
                    FriendlyException.ThrowIfNotFile(inPath);
                    byte[] bytes = File.ReadAllBytes(inPath);

                    var file = new Zero3.File(finalName, bytes);
                    z3.Files.Add(file);
                }
            }

            // Write
            string outPath = Path.Combine(targetDir, zero3Name);
            BackupZero3(outPath);
            z3.Write(outPath);
        }

        private static void BackupZero3(string outPath)
        {
            // Get usable output directory
            string dirName = outPath.Replace('.', '-');
            string outDir = $"{dirName}-bak";
            int outDirIndex = 0;
            while (File.Exists(outDir))
                outDir = $"{dirName}{outDirIndex++}-bak";

            // Create output directory
            Directory.CreateDirectory(outDir);

            // Backup parts
            int index = 0;
            string nextPath = outPath;
            while (File.Exists(nextPath))
            {
                string nextOutPath = Path.Combine(outDir, Path.GetFileName(nextPath));
                if (!File.Exists(nextOutPath))
                {
                    File.Move(nextPath, nextOutPath);
                }

                index++;
                nextPath = Path.ChangeExtension(outPath, $"{index:D3}");
            }
        }
    }
}
