using SoulsFormats;
using System;
using System.Xml;

namespace YabberExtended.Helpers
{
    internal static class DcxHelper
    {
        public static void WriteCompressionInfo(XmlWriter xw, DCX.CompressionInfo compression, string compressionNodeName = "compression")
        {
            xw.WriteElementString(compressionNodeName, compression.Type.ToString());
            switch (compression.Type)
            {
                case DCX.Type.DCX_DFLT:
                    if (compression is DCX.DcxDfltCompressionInfo dflt)
                    {
                        xw.WriteElementString("compression_dflt_unk04", dflt.Unk04.ToString());
                        xw.WriteElementString("compression_dflt_unk10", dflt.Unk10.ToString());
                        xw.WriteElementString("compression_dflt_unk14", dflt.Unk14.ToString());
                        xw.WriteElementString("compression_dflt_unk30", dflt.Unk30.ToString());
                        xw.WriteElementString("compression_dflt_unk38", dflt.Unk38.ToString());
                    }
                    else
                    {
                        throw new NotSupportedException($"{nameof(DCX.Type)} is {DCX.Type.DCX_DFLT} yet {nameof(DCX.CompressionInfo)} is not {nameof(DCX.DcxDfltCompressionInfo)}.");
                    }
                    break;
                case DCX.Type.DCX_KRAK:
                    if (compression is DCX.DcxKrakCompressionInfo krak)
                        xw.WriteElementString("compression_krak_compressionlevel", krak.CompressionLevel.ToString());
                    else
                        throw new NotSupportedException($"{nameof(DCX.Type)} is {DCX.Type.DCX_KRAK} yet {nameof(DCX.CompressionInfo)} is not {nameof(DCX.DcxKrakCompressionInfo)}.");
                    break;
            }
        }

        public static DCX.CompressionInfo ReadCompressionInfo(XmlNode? node, string compressionNodeName = "compression", string compressionDefault = "None")
        {
            string compressionText = node?.SelectSingleNode(compressionNodeName)?.InnerText ?? compressionDefault;

            DCX.CompressionInfo compressionInfo;
            DCX.Type compression;
            switch (compressionText)
            {
                case "DCX_DFLT_10000_24_9":
                    compressionInfo = new DCX.DcxDfltCompressionInfo(DCX.DfltCompressionPreset.DCX_DFLT_10000_24_9);
                    break;
                case "DCX_DFLT_10000_44_9":
                    compressionInfo = new DCX.DcxDfltCompressionInfo(DCX.DfltCompressionPreset.DCX_DFLT_10000_44_9);
                    break;
                case "DCX_DFLT_11000_44_8":
                    compressionInfo = new DCX.DcxDfltCompressionInfo(DCX.DfltCompressionPreset.DCX_DFLT_11000_44_8);
                    break;
                case "DCX_DFLT_11000_44_9":
                    compressionInfo = new DCX.DcxDfltCompressionInfo(DCX.DfltCompressionPreset.DCX_DFLT_11000_44_9);
                    break;
                case "DCX_DFLT_11000_44_9_15":
                    compressionInfo = new DCX.DcxDfltCompressionInfo(DCX.DfltCompressionPreset.DCX_DFLT_11000_44_9_15);
                    break;
                default:
                    compression = (DCX.Type)Enum.Parse(typeof(DCX.Type), compressionText);

                    switch (compression)
                    {
                        case DCX.Type.Unknown:
                            compressionInfo = new DCX.UnkCompressionInfo();
                            break;
                        case DCX.Type.None:
                            compressionInfo = new DCX.NoCompressionInfo();
                            break;
                        case DCX.Type.Zlib:
                            compressionInfo = new DCX.ZlibCompressionInfo();
                            break;
                        case DCX.Type.DCP_EDGE:
                            compressionInfo = new DCX.DcpEdgeCompressionInfo();
                            break;
                        case DCX.Type.DCP_DFLT:
                            compressionInfo = new DCX.DcpDfltCompressionInfo();
                            break;
                        case DCX.Type.DCX_EDGE:
                            compressionInfo = new DCX.DcxEdgeCompressionInfo();
                            break;
                        case DCX.Type.DCX_DFLT:
                            _ = int.TryParse(node?.SelectSingleNode("compression_dflt_unk04")?.InnerText ?? "0", out int unk04);
                            _ = int.TryParse(node?.SelectSingleNode("compression_dflt_unk10")?.InnerText ?? "0", out int unk10);
                            _ = int.TryParse(node?.SelectSingleNode("compression_dflt_unk14")?.InnerText ?? "0", out int unk14);
                            _ = byte.TryParse(node?.SelectSingleNode("compression_dflt_unk30")?.InnerText ?? "0", out byte unk30);
                            _ = byte.TryParse(node?.SelectSingleNode("compression_dflt_unk38")?.InnerText ?? "0", out byte unk38);
                            compressionInfo = new DCX.DcxDfltCompressionInfo(unk04, unk10, unk14, unk30, unk38);
                            break;
                        case DCX.Type.DCX_KRAK:
                            _ = byte.TryParse(node?.SelectSingleNode("compression_krak_compressionlevel")?.InnerText ?? "0", out byte compressionLevel);
                            compressionInfo = new DCX.DcxKrakCompressionInfo(compressionLevel);
                            break;
                        case DCX.Type.DCX_ZSTD:
                            compressionInfo = new DCX.DcxZstdCompressionInfo();
                            break;
                        default:
                            throw new NotSupportedException($"{nameof(DCX.Type)} {compression} is not supported for: {nameof(ReadCompressionInfo)}");
                    }
                    break;
            }

            return compressionInfo;
        }
    }
}
