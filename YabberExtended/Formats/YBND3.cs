using SoulsFormats;
using System;
using System.IO;
using System.Xml;
using YabberExtended.Helpers;

namespace YabberExtended
{
    static class YBND3
    {
        public static void Unpack(this BND3Reader bnd, string sourceName, string targetDir, IProgress<float> progress)
        {
            Directory.CreateDirectory(targetDir);
            var xws = new XmlWriterSettings();
            xws.Indent = true;
            var xw = XmlWriter.Create($"{targetDir}\\_yabber-bnd3.xml", xws);
            xw.WriteStartElement("bnd3");

            xw.WriteElementString("filename", sourceName);
            xw.WriteElementString("compression", bnd.Compression.ToString());
            xw.WriteElementString("version", bnd.Version);
            xw.WriteElementString("format", bnd.Format.ToString());
            xw.WriteElementString("bigendian", bnd.BigEndian.ToString());
            xw.WriteElementString("bitbigendian", bnd.BitBigEndian.ToString());
            xw.WriteElementString("unk18", $"0x{bnd.Unk18:X}");
            xw.WriteElementString("writefileheadersend", bnd.WriteFileHeadersEnd.ToString());
            YBinder.WriteBinderFiles(bnd, xw, targetDir, progress);

            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string sourceDir, string targetDir)
        {
            var bnd = new BND3();
            var xml = new XmlDocument();
            xml.Load($"{sourceDir}\\_yabber-bnd3.xml");

            if (xml.SelectSingleNode("bnd3/filename") == null)
                throw new FriendlyException("Missing filename tag.");

            string filename = xml.SelectSingleNode("bnd3/filename").InnerText;
            string strCompression = xml.SelectSingleNode("bnd3/compression")?.InnerText ?? "None";
            bnd.Version = xml.SelectSingleNode("bnd3/version")?.InnerText ?? "07D7R6";
            string strFormat = xml.SelectSingleNode("bnd3/format")?.InnerText ?? "IDs, Names1, Names2, Compression";
            string strBigEndian = xml.SelectSingleNode("bnd3/bigendian")?.InnerText ?? "False";
            string strBitBigEndian = xml.SelectSingleNode("bnd3/bitbigendian")?.InnerText ?? "False";
            string strUnk18 = xml.SelectSingleNode("bnd3/unk18")?.InnerText ?? "0x0";
            string strWriteFileHeadersEnd = xml.SelectSingleNode("bnd3/writefileheadersend")?.InnerText ?? "True";

            bnd.Compression = DcxHelper.BuildCompressionInfo(strCompression);

            try
            {
                bnd.Format = (Binder.Format)Enum.Parse(typeof(Binder.Format), strFormat);
            }
            catch
            {
                throw new FriendlyException($"Could not parse format: {strFormat}\nFormat must be a comma-separated list of flags.");
            }

            if (!bool.TryParse(strBigEndian, out bool bigEndian))
                throw new FriendlyException($"Could not parse big-endianness: {strBigEndian}\nBig-endianness must be true or false.");
            bnd.BigEndian = bigEndian;

            if (!bool.TryParse(strBitBigEndian, out bool bitBigEndian))
                throw new FriendlyException($"Could not parse bit big-endianness: {strBitBigEndian}\nBit big-endianness must be true or false.");
            bnd.BitBigEndian = bitBigEndian;

            try
            {
                bnd.Unk18 = Convert.ToInt32(strUnk18, 16);
            }
            catch
            {
                throw new FriendlyException($"Could not parse unk18: {strUnk18}\nUnk18 must be a hex value.");
            }

            if (!bool.TryParse(strWriteFileHeadersEnd, out bool writeFileHeadersEnd))
                writeFileHeadersEnd = true;
            bnd.WriteFileHeadersEnd = writeFileHeadersEnd;

            XmlNodeList? fileNodes = xml.SelectNodes("bnd3/files/file");
            if (fileNodes != null)
                YBinder.ReadBinderFiles(bnd, fileNodes, sourceDir);

            string outPath = $"{targetDir}\\{filename}";
            YabberUtil.BackupFile(outPath);
            bnd.Write(outPath);
        }
    }
}
