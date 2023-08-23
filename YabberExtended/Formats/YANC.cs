using SoulsFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace YabberExtended.Formats
{
    static class YANC
    {
        public static void Unpack(this ANC anc, string filename, string targetDir)
        {
            Directory.CreateDirectory(targetDir);
            var xws = new XmlWriterSettings();
            xws.Indent = true;
            var xw = XmlWriter.Create($"{targetDir}\\_yabber-anc.xml", xws);
            xw.WriteStartElement("ANC");
            xw.WriteElementString("AncName", filename);
            xw.WriteElementString("AniEditString", anc.AniEdit);
            xw.WriteElementString("AneName", anc.AneName);
            xw.WriteEndElement();
            xw.Close();
            string outPath = $"{targetDir}\\{anc.AneName}";
            File.WriteAllBytes(outPath, anc.AneData);
        }

        public static void Repack(string sourceDir, string targetDir)
        {
            ANC anc = new ANC();
            XmlDocument xml = new XmlDocument();
            xml.Load($"{sourceDir}\\_yabber-anc.xml");
            string ancName = xml.SelectSingleNode("ANC/AncName").InnerText;
            string aniEditStr = xml.SelectSingleNode("ANC/AniEditString").InnerText;
            string aneName = xml.SelectSingleNode("ANC/AneName").InnerText;

            if (!ancName.EndsWith(".anc"))
                Console.WriteLine("Warning: ANC file name does not end with \".anc\", it may not be read, proceeding.");
            if (aniEditStr != "#ANIEDIT")
                Console.WriteLine("Warning: AniEditString is always set to \"#ANIEDIT\", AniEditString does not match, proceeding.");
            if (!aneName.EndsWith(".ane"))
                Console.WriteLine("Warning: ANE file name does not end with \".ane\", it may not be read, proceeding.");
            if (aniEditStr.Length > 0x10)
                throw new FriendlyException("AniEditString must not be longer than 16 characters.");
            if (aneName.Length > 0x10)
                throw new FriendlyException("Ane file name must not be longer than 16 characters");
            if (!File.Exists($"{sourceDir}\\{aneName}"))
                throw new FriendlyException("Ane file not found, please make sure it exists and AneName is correct.");

            anc.AniEdit = aniEditStr;
            anc.AneName = aneName;
            anc.AneData = File.ReadAllBytes($"{sourceDir}\\{aneName}");

            string outPath = $"{targetDir}\\{ancName}";
            YBUtil.Backup(outPath);
            anc.Write(outPath);
        }
    }
}
