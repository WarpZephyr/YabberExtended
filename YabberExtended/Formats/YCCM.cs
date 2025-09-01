using SoulsFormats;
using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using YabberExtended.Extensions.Xml;
using YabberExtended.Helpers;

namespace YabberExtended
{
    static class YCCM
    {
        public static void Unpack(this CCM ccm, string sourceFile)
        {
            var xws = new XmlWriterSettings();
            xws.Indent = true;
            using XmlWriter xw = XmlWriter.Create($"{sourceFile}.xml", xws);
            xw.WriteStartElement("ccm");
            DcxHelper.WriteCompressionInfo(xw, ccm.Compression);
            xw.WriteElementString("version", ccm.Version.ToString());
            xw.WriteElementString("fullwidth", ccm.FullWidth.ToString());
            xw.WriteElementString("texwidth", ccm.TexWidth.ToString());
            xw.WriteElementString("texheight", ccm.TexHeight.ToString());
            xw.WriteElementString("unk0E", ccm.Unk0E.ToString());
            xw.WriteElementString("unk1C", ccm.Unk1C.ToString());
            xw.WriteElementString("unk1D", ccm.Unk1D.ToString());
            xw.WriteElementString("texcount", ccm.TexCount.ToString());
            xw.WriteStartElement("glyphs");
            foreach (var pair in ccm.Glyphs)
            {
                var code = pair.Key;
                var glyph = pair.Value;
                xw.WriteStartElement("glyph");
                xw.WriteAttributeString("code", $"0x{code:x8}");
                xw.WriteAttributeString("uv0", ToStringVector2(glyph.UV1));
                xw.WriteAttributeString("uv1", ToStringVector2(glyph.UV2));
                xw.WriteAttributeString("padding", glyph.PreSpace.ToString());
                xw.WriteAttributeString("width", glyph.Width.ToString());
                xw.WriteAttributeString("advance", glyph.Advance.ToString());
                xw.WriteAttributeString("texture", glyph.TexIndex.ToString());
                xw.WriteEndElement();
            }
            xw.WriteEndElement();
            xw.WriteEndElement();
        }

        public static void Repack(string sourceFile)
        {
            CCM ccm = new CCM();
            XmlDocument xml = new XmlDocument();
            xml.Load(sourceFile);
            ccm.Compression = DcxHelper.ReadCompressionInfo(xml.SelectSingleNode("ccm"));
            ccm.Version = xml.ReadEnum<CCM.CCMVer>("ccm/version");
            ccm.FullWidth = xml.ReadInt16("ccm/fullwidth");
            ccm.TexWidth = xml.ReadInt16("ccm/texwidth");
            ccm.TexHeight = xml.ReadInt16("ccm/texheight");
            ccm.Unk0E = xml.ReadInt16("ccm/unk0E");
            ccm.Unk1C = xml.ReadByte("ccm/unk1C");
            ccm.Unk1D = xml.ReadByte("ccm/unk1D");
            ccm.TexCount = xml.ReadByte("ccm/texcount");

            var glyphNodes = xml.SelectNodes("ccm/glyphs/glyph");
            if (glyphNodes != null)
            {
                foreach (XmlNode glyphNode in glyphNodes)
                {
                    var attributes = glyphNode.GetAttributesOrThrow();
                    var codeStr = attributes.ReadString("code");
                    var codeStrOriginal = codeStr;
                    if (codeStr.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                        codeStr = codeStr[2..];

                    if (!int.TryParse(codeStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int code))
                        throw new FriendlyException($"Couldn't parse {nameof(code)} from value: \"{codeStrOriginal}\"");

                    var uv0 = ParseVector2(attributes.ReadString("uv0"));
                    var uv1 = ParseVector2(attributes.ReadString("uv1"));
                    var padding = attributes.ReadInt16("padding");
                    var width = attributes.ReadInt16("width");
                    var advance = attributes.ReadInt16("advance");
                    var texture = attributes.ReadInt16("texture");

                    var glyph = new CCM.Glyph(uv0, uv1, padding, width, advance, texture);
                    ccm.Glyphs.Add(code, glyph);
                }
            }

            string outPath = sourceFile.Replace(".ccm.xml", ".ccm");
            YabberUtil.BackupFile(outPath);
            ccm.Write(outPath);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string ToStringVector2(Vector2 value)
            => $"{value.X},{value.Y}";

        private static Vector2 ParseVector2(string str)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if (char.IsDigit(str[i]) || str[i] == ',' || str[i] == '.')
                {
                    sb.Append(str[i]);
                }
            }

            string[] split = str.Split(',');
            float x = 0f;
            if (split.Length > 0)
            {
                string xStr = split[0];
                if (!float.TryParse(xStr, out x))
                {
                    return default;
                }
            }

            float y = 0f;
            if (split.Length > 1)
            {
                string yStr = split[1];
                if (!float.TryParse(yStr, out y))
                {
                    return default;
                }
            }

            return new Vector2(x, y);
        }
    }
}
