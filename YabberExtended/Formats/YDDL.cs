using SoulsFormats;
using System.IO;
using System.Xml;

namespace YabberExtended
{
    static class YDDL
    {
        public static void Unpack(this DDL ddl, string targetDir)
        {
            Directory.CreateDirectory(targetDir);
            var xws = new XmlWriterSettings();
            xws.Indent = true;
            var xw = XmlWriter.Create(Path.Combine(targetDir, "_yabber-ddl.xml"), xws);
            xw.WriteStartElement("ddl");

            xw.WriteElementString("name", ddl.Name);
            xw.WriteElementString("version", ddl.Version.ToString());

            xw.WriteStartElement("dds_entries");
            foreach (var entry in ddl.DDSEntries)
            {
                xw.WriteElementString("name", entry.Name);
                File.WriteAllBytes(Path.Combine(targetDir, entry.Name), entry.Bytes);
            }
            xw.WriteEndElement();

            xw.WriteStartElement("prm_entries");
            foreach (var entry in ddl.PRMEntries)
            {
                xw.WriteElementString("name", entry.Name);
                File.WriteAllBytes(Path.Combine(targetDir, entry.Name), entry.Bytes);
            }
            xw.WriteEndElement();

            xw.WriteEndElement();
            xw.Close();
        }
    }
}
