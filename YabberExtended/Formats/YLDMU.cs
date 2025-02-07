using SoulsFormats;
using System;
using System.IO;
using System.Xml;
using YabberExtended.Extensions;
using YabberExtended.Extensions.Hex;
using YabberExtended.Extensions.Value;
using YabberExtended.Extensions.Xml;
using static YabberExtended.YabberUtil;

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

            string bhdFilename = xml.ReadStringOrThrowIfWhiteSpace("ldmu/bhd_filename");
            string bndFilename = xml.ReadStringOrThrowIfWhiteSpace("ldmu/bnd_filename");
            bhd.Unk04 = xml.ReadInt32("ldmu/unk04");

            var fileNodes = xml.SelectNodes("ldmu/files/file");
            if (fileNodes != null)
            {
                foreach (XmlNode fileNode in fileNodes)
                {
                    var file = new LDMU.File();
                    string id = fileNode.GetXmlValueOrContents("id");
                    bool compress = fileNode.ReadBoolean("compress");
                    byte[] unk10 = fileNode.ReadStringOrThrowIfWhiteSpace("unk10").HexToBytes();
                    if (unk10.Length != 24)
                    {
                        throw new FriendlyException($"{nameof(LDMU.File.Unk10)} must be {24} bytes long.");
                    }

                    file.Compress = compress;
                    file.ID = id.ToInt32();
                    file.Unk10 = unk10;

                    string inPath = fileNode.GetFilePathNameOrUseID(sourceDir, id);
                    FriendlyException.ThrowIfNotFile(inPath);
                    file.Bytes = File.ReadAllBytes(inPath);
                    bhd.Files.Add(file);
                }
            }

            string bhdPath = Path.Combine(targetDir, bhdFilename);
            string bndPath = Path.Combine(targetDir, bndFilename);
            BackupFile(bhdPath);
            BackupFile(bndPath);
            bhd.Write(bhdPath, bndPath);
        }
    }
}
