using System;
using System.Globalization;
using System.IO;
using System.Xml;
using SoulsFormats.ACE3;
using YabberExtended.Extensions;
using YabberExtended.Extensions.Value;
using YabberExtended.Extensions.Xml;
using static YabberExtended.YabberUtil;

namespace YabberExtended
{
    static class YBNDACE3
    {
        public static void Unpack(this BND bnd, string sourceName, string targetDir, IProgress<float> progress)
        {
            Directory.CreateDirectory(targetDir);
            var xws = new XmlWriterSettings();
            xws.Indent = true;
            var xw = XmlWriter.Create(Path.Combine(targetDir, "_yabber-bnd_ace3.xml"), xws);
            xw.WriteStartElement("bnd_ace3");
            xw.WriteElementString("binder_name", sourceName);
            xw.WriteElementString("lite", bnd.Lite.ToString());
            xw.WriteElementString("flag1", bnd.Flag1.ToString());
            xw.WriteElementString("flag2", bnd.Flag2.ToString());

            xw.WriteStartElement("files");
            for (int i = 0; i < bnd.Files.Count; i++)
            {
                xw.WriteStartElement("file");
                var file = bnd.Files[i];
                string strID = file.ID.ToString();

                xw.WriteElementString("id", strID);
                xw.WriteEndElement();

                string outPath = Path.Combine(targetDir, strID);
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
            xml.Load(Path.Combine(sourceDir, "_yabber-bnd_ace3.xml"));

            string binderName = xml.ReadStringOrThrowIfWhiteSpace("bnd_ace3/binder_name");
            bnd.Lite = xml.ReadBoolean("bnd_ace3/lite");
            bnd.Flag1 = xml.ReadByte("bnd_ace3/flag1");
            bnd.Flag2 = xml.ReadByte("bnd_ace3/flag2");

            var fileNodes = xml.SelectNodes("bnd_ace3/files/file");
            if (fileNodes != null)
            {
                foreach (XmlNode fileNode in fileNodes)
                {
                    string id = fileNode.GetXmlValueOrContents("id");
                    string inPath = fileNode.GetFilePathNameOrUseID(sourceDir, id);
                    FriendlyException.ThrowIfNotFile(inPath);
                    bnd.Files.Add(new BND.File(id.ToInt32(), File.ReadAllBytes(inPath)));
                }
            }

            string outPath = Path.Combine(targetDir, binderName);
            BackupFile(outPath);

            bnd.Write(outPath);
        }
    }
}
