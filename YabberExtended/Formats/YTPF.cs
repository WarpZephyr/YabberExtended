using SoulsFormats;
using System;
using System.IO;
using System.Xml;
using YabberExtended.Helpers;

namespace YabberExtended
{
    static class YTPF
    {
        public static void Unpack(this TPF tpf, string sourceName, string targetDir, IProgress<float> progress)
        {
            Directory.CreateDirectory(targetDir);
            var xws = new XmlWriterSettings();
            xws.Indent = true;
            var xw = XmlWriter.Create($"{targetDir}\\_yabber-tpf.xml", xws);
            xw.WriteStartElement("tpf");

            xw.WriteElementString("filename", sourceName);
            xw.WriteElementString("compression", tpf.Compression.ToString());
            xw.WriteElementString("platform", tpf.Platform.ToString());
            xw.WriteElementString("encoding", $"0x{tpf.Encoding:X2}");
            xw.WriteElementString("flag2", $"0x{tpf.Flag2:X2}");

            xw.WriteStartElement("textures");
            for (int i = 0; i < tpf.Textures.Count; i++)
            {
                TPF.Texture texture = tpf.Textures[i];
                xw.WriteStartElement("texture");
                xw.WriteElementString("name", texture.Name + ".dds");
                xw.WriteElementString("format", texture.Format.ToString());
                xw.WriteElementString("flags1", $"0x{texture.Flags1:X2}");

                if (tpf.Platform != TPF.TPFPlatform.PC)
                {
                    //xw.WriteElementString("width", texture.Header.Width.ToString());
                    //xw.WriteElementString("height", texture.Header.Height.ToString());
                    if (tpf.Platform == TPF.TPFPlatform.PS3)
                    {
                        xw.WriteElementString("Unk1", texture.Header.Unk1.ToString());
                        if (tpf.Flag2 != 0)
                        {
                            xw.WriteElementString("Unk2", texture.Header.Unk2.ToString());
                        }
                    }
                    else if (tpf.Platform == TPF.TPFPlatform.PS4 || tpf.Platform == TPF.TPFPlatform.Xbone)
                    {
                        xw.WriteElementString("TextureCount", texture.Header.TextureCount.ToString());
                        xw.WriteElementString("Unk2", texture.Header.Unk2.ToString());
                    }
                }

                if (texture.FloatStruct != null)
                {
                    xw.WriteStartElement("FloatStruct");
                    xw.WriteAttributeString("Unk00", texture.FloatStruct.Unk00.ToString());
                    foreach (float value in texture.FloatStruct.Values)
                    {
                        xw.WriteElementString("Value", value.ToString());
                    }
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();

                File.WriteAllBytes($"{targetDir}\\{texture.Name}.dds", texture.Headerize());
                progress.Report((float)i / tpf.Textures.Count);
            }
            xw.WriteEndElement();

            xw.WriteEndElement();
            xw.Close();
        }

        public static void Repack(string sourceDir, string targetDir)
        {
            TPF tpf = new TPF();
            XmlDocument xml = new XmlDocument();
            xml.Load($"{sourceDir}\\_yabber-tpf.xml");

            string filename = xml.SelectSingleNode("tpf/filename").InnerText;
            string compressionText = xml.SelectSingleNode("tpf/compression")?.InnerText ?? "None";
            tpf.Compression = DcxHelper.BuildCompressionInfo(compressionText);
            Enum.TryParse(xml.SelectSingleNode("tpf/platform")?.InnerText ?? "None", out TPF.TPFPlatform platform);
            tpf.Platform = platform;
            tpf.Encoding = Convert.ToByte(xml.SelectSingleNode("tpf/encoding").InnerText, 16);
            tpf.Flag2 = Convert.ToByte(xml.SelectSingleNode("tpf/flag2").InnerText, 16);

            foreach (XmlNode texNode in xml.SelectNodes("tpf/textures/texture"))
            {
                string fileName = texNode.SelectSingleNode("name").InnerText;
                string name = Path.GetFileNameWithoutExtension(fileName);
                byte format = Convert.ToByte(texNode.SelectSingleNode("format").InnerText);
                byte flags1 = Convert.ToByte(texNode.SelectSingleNode("flags1").InnerText, 16);

                short width = 0;
                short height = 0;
                int unk1 = 0;
                int unk2 = 0;
                int textureCount = 0;
                if (tpf.Platform != TPF.TPFPlatform.PC)
                {
                    DDS dds = new DDS(File.ReadAllBytes($"{sourceDir}\\{fileName}"));
                    width = (short)dds.dwWidth;
                    height = (short)dds.dwHeight;
                    if (tpf.Platform == TPF.TPFPlatform.PS3)
                    {
                        unk1 = int.Parse(texNode.SelectSingleNode("Unk1").InnerText);
                        if (tpf.Flag2 != 0)
                        {
                            unk2 = int.Parse(texNode.SelectSingleNode("Unk2").InnerText);
                        }
                    }
                    else if (tpf.Platform == TPF.TPFPlatform.PS4 || tpf.Platform == TPF.TPFPlatform.Xbone)
                    {
                        textureCount = int.Parse(texNode.SelectSingleNode("TextureCount").InnerText);
                        unk2 = int.Parse(texNode.SelectSingleNode("Unk2").InnerText);
                    }
                }

                TPF.FloatStruct floatStruct = null;
                XmlNode floatsNode = texNode.SelectSingleNode("FloatStruct");
                if (floatsNode != null)
                {
                    floatStruct = new TPF.FloatStruct();
                    floatStruct.Unk00 = int.Parse(floatsNode.Attributes["Unk00"].InnerText);
                    foreach (XmlNode valueNode in floatsNode.SelectNodes("Value"))
                        floatStruct.Values.Add(float.Parse(valueNode.InnerText));
                }

                byte[] bytes = File.ReadAllBytes($"{sourceDir}\\{name}.dds");
                var texture = new TPF.Texture(name, format, flags1, bytes, tpf.Platform);

                if (tpf.Platform != TPF.TPFPlatform.PC)
                {
                    texture.Header = new TPF.TexHeader();
                    texture.Header.Width = width;
                    texture.Header.Height = height;
                    if (tpf.Platform == TPF.TPFPlatform.PS3)
                    {
                        texture.Header.Unk1 = unk1;
                        if (tpf.Flag2 != 0)
                        {
                            texture.Header.Unk2 = unk2;
                        }
                    }
                    else if (tpf.Platform == TPF.TPFPlatform.PS4 || tpf.Platform == TPF.TPFPlatform.Xbone)
                    {
                        texture.Header.TextureCount = textureCount;
                        texture.Header.Unk2 = unk2;
                    }

                    //texture.Bytes = texture.Deheaderize();
                }

                texture.FloatStruct = floatStruct;
                tpf.Textures.Add(texture);
            }

            string outPath = $"{targetDir}\\{filename}";
            YabberUtil.BackupFile(outPath);
            tpf.Write(outPath);
        }
    }
}
