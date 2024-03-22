using SoulsFormats;
using System;
using System.IO;
using System.Xml;
using static YabberExtended.YBUtil;

namespace YabberExtended
{
    static class YLDMU
    {
        public static void Unpack(this LDMU bhd, string bhdName, string bndName, string targetDir, IProgress<float> progress)
        {
            Directory.CreateDirectory(targetDir);
            var xws = new XmlWriterSettings();
            xws.Indent = true;
            var xw = XmlWriter.Create(Path.Combine(targetDir, "_yabber-ldmu.xml"), xws);
            xw.WriteStartElement("ldmu");
            xw.WriteElementString("bhd_filename", bhdName);
            xw.WriteElementString("bnd_filename", bndName);
            xw.WriteElementString("unk04", bhd.Unk04.ToString());

            xw.WriteStartElement("files");
            int count = bhd.Files.Count;
            for (int i = 0; i < count; i++)
            {
                var file = bhd.Files[i];

                string strID = file.ID.ToString();
                xw.WriteStartElement("file");
                xw.WriteElementString("compress", file.Compress.ToString());
                xw.WriteElementString("id", strID);
                xw.WriteElementString("unk10", file.Unk10.ToHexString());
                xw.WriteEndElement();

                string outPath = Path.Combine(targetDir, strID);
                File.WriteAllBytes(outPath, file.Bytes);
                progress.Report((float)i / count);
            }
            xw.WriteEndElement();

            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string sourceDir, string targetDir)
        {
            var bhd = new LDMU();
            var xml = new XmlDocument();
            xml.Load(Path.Combine(sourceDir, "_yabber-ldmu.xml"));

            string bhdFilename = xml.SelectSingleNode("ldmu/bhd_filename").InnerText;
            string bndFilename = xml.SelectSingleNode("ldmu/bnd_filename").InnerText;
            bhd.Unk04 = FieldToInt32(xml.SelectSingleNode("ldmu/unk04").InnerText, "unk04");

            var filesNode = xml.SelectSingleNode("ldmu/files");
            if (filesNode != null)
            {
                foreach (XmlNode fileNode in filesNode.SelectNodes("file"))
                {
                    var file = new LDMU.File();
                    string strID = fileNode.SelectSingleNode("id")?.InnerText;
                    bool compress = FieldToBool(fileNode.SelectSingleNode("compress").InnerText, "compress");
                    int id = FieldToInt32(strID, "id");
                    byte[] unk10 = FieldToString(fileNode.SelectSingleNode("unk10").InnerText, "unk10").HexToBytes();
                    if (unk10.Length != 24)
                    {
                        throw new FriendlyException("unk10 must be 24 bytes long.");
                    }

                    file.Compress = compress;
                    file.ID = id;
                    file.Unk10 = unk10;

                    string inPath = Path.Combine(sourceDir, strID);
                    if (!File.Exists(inPath))
                    {
                        throw new FriendlyException($"File not found: {inPath}");
                    }

                    file.Bytes = File.ReadAllBytes(inPath);
                    bhd.Files.Add(file);
                }
            }

            string bhdPath = Path.Combine(targetDir, bhdFilename);
            string bndPath = Path.Combine(targetDir, bndFilename);
            Backup(bhdPath);
            Backup(bndPath);
            bhd.Write(bhdPath, bndPath);
        }
    }
}
