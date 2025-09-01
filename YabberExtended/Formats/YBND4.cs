using SoulsFormats;
using System;
using System.IO;
using System.Xml;
using YabberExtended.Helpers;

namespace YabberExtended
{
    static class YBND4
    {
        public static void Unpack(this BND4Reader bnd, string sourceName, string targetDir, IProgress<float> progress)
        {
            Directory.CreateDirectory(targetDir);
            var xws = new XmlWriterSettings();
            xws.Indent = true;
            var xw = XmlWriter.Create($"{targetDir}\\_yabber-bnd4.xml", xws);
            xw.WriteStartElement("bnd4");

            xw.WriteElementString("filename", sourceName);
            DcxHelper.WriteCompressionInfo(xw, bnd.Compression);
            xw.WriteElementString("version", bnd.Version);
            xw.WriteElementString("format", bnd.Format.ToString());
            xw.WriteElementString("bigendian", bnd.BigEndian.ToString());
            xw.WriteElementString("bitbigendian", bnd.BitBigEndian.ToString());
            xw.WriteElementString("unicode", bnd.Unicode.ToString());
            xw.WriteElementString("extended", $"0x{bnd.Extended:X2}");
            xw.WriteElementString("unk04", bnd.Unk04.ToString());
            xw.WriteElementString("unk05", bnd.Unk05.ToString());
            YBinder.WriteBinderFiles(bnd, xw, targetDir, progress);
            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string sourceDir, string targetDir)
        {
            var bnd = new BND4();
            var xml = new XmlDocument();
            xml.Load($"{sourceDir}\\_yabber-bnd4.xml");

            string filename = xml.SelectSingleNode("bnd4/filename").InnerText;
            bnd.Compression = DcxHelper.ReadCompressionInfo(xml.SelectSingleNode("bnd4"));
            bnd.Version = xml.SelectSingleNode("bnd4/version").InnerText;
            bnd.Format = (Binder.Format)Enum.Parse(typeof(Binder.Format), xml.SelectSingleNode("bnd4/format").InnerText);
            bnd.BigEndian = bool.Parse(xml.SelectSingleNode("bnd4/bigendian").InnerText);
            bnd.BitBigEndian = bool.Parse(xml.SelectSingleNode("bnd4/bitbigendian").InnerText);
            bnd.Unicode = bool.Parse(xml.SelectSingleNode("bnd4/unicode").InnerText);
            bnd.Extended = Convert.ToByte(xml.SelectSingleNode("bnd4/extended").InnerText, 16);
            bnd.Unk04 = bool.Parse(xml.SelectSingleNode("bnd4/unk04").InnerText);
            bnd.Unk05 = bool.Parse(xml.SelectSingleNode("bnd4/unk05").InnerText);
            XmlNodeList? fileNodes = xml.SelectNodes("bnd4/files/file");
            if (fileNodes != null)
                YBinder.ReadBinderFiles(bnd, fileNodes, sourceDir);

            string outPath = $"{targetDir}\\{filename}";
            YabberUtil.BackupFile(outPath);
            bnd.Write(outPath);
        }
    }
}
