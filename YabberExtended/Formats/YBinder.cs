﻿using SoulsFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using YabberExtended.Helpers;

namespace YabberExtended
{
    static class YBinder
    {
        public static void WriteBinderFiles(BinderReader bnd, XmlWriter xw, string targetDir, IProgress<float> progress)
        {
            xw.WriteStartElement("files");
            var pathCounts = new Dictionary<string, int>();
            for (int i = 0; i < bnd.Files.Count; i++)
            {
                BinderFileHeader file = bnd.Files[i];

                string root = string.Empty;
                string path;
                if (Binder.HasNames(bnd.Format))
                {
                    path = YabberUtil.SplitBinderRootFromPath(file.Name, out root);
                }
                else if (Binder.HasIDs(bnd.Format))
                {
                    path = file.ID.ToString();
                }
                else
                {
                    path = i.ToString();
                }

                xw.WriteStartElement("file");
                xw.WriteElementString("flags", file.Flags.ToString());

                if (Binder.HasIDs(bnd.Format))
                    xw.WriteElementString("id", file.ID.ToString());

                if (root != string.Empty)
                    xw.WriteElementString("root", root);

                xw.WriteElementString("path", path);

                string suffix = string.Empty;
                if (pathCounts.TryGetValue(path, out int value))
                {
                    pathCounts[path] = ++value;
                    suffix = $" ({value})";
                    xw.WriteElementString("suffix", suffix);
                }
                else
                {
                    pathCounts[path] = 1;
                }

                if (file.Compression.Type != DCX.Type.Zlib)
                {
                    DcxHelper.WriteCompressionInfo(xw, file.Compression, "compression_type");
                }

                xw.WriteEndElement();

                byte[] bytes = bnd.ReadFile(file);
                string outPath = $@"{targetDir}\{Path.GetDirectoryName(path)}\{Path.GetFileNameWithoutExtension(path)}{suffix}{Path.GetExtension(path)}";
                Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                File.WriteAllBytes(outPath, bytes);
                progress.Report((float)i / bnd.Files.Count);
            }
            xw.WriteEndElement();
        }

        public static void ReadBinderFiles(IBinder bnd, XmlNodeList fileNodes, string sourceDir)
        {
            foreach (XmlNode fileNode in fileNodes)
            {
                if (fileNode.SelectSingleNode("path") == null)
                    throw new FriendlyException("File node missing path tag.");

                string strFlags = fileNode.SelectSingleNode("flags")?.InnerText ?? "Flag1";
                string strID = fileNode.SelectSingleNode("id")?.InnerText ?? "-1";
                string root = fileNode.SelectSingleNode("root")?.InnerText ?? "";
                string path = fileNode.SelectSingleNode("path").InnerText;
                string suffix = fileNode.SelectSingleNode("suffix")?.InnerText ?? "";
                string name = root + path;

                if (!Enum.TryParse(strFlags, out Binder.FileFlags flags))
                    throw new FriendlyException($"Could not parse file flags: {strFlags}\nFlags must be comma-separated list of flags.");

                if (!int.TryParse(strID, out int id))
                    throw new FriendlyException($"Could not parse file ID: {strID}\nID must be a 32-bit signed integer.");

                string inPath = $@"{sourceDir}\{Path.GetDirectoryName(path)}\{Path.GetFileNameWithoutExtension(path)}{suffix}{Path.GetExtension(path)}";
                FriendlyException.ThrowIfNotFile(inPath);

                byte[] bytes = File.ReadAllBytes(inPath);
                bnd.Files.Add(new BinderFile(flags, id, name, bytes)
                {
                    CompressionInfo = DcxHelper.ReadCompressionInfo(fileNode, "compression_type", DCX.Type.Zlib.ToString())
                });
            }
        }
    }
}
