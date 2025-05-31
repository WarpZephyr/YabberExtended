using SoulsFormats;
using System;

namespace YabberExtended.Helpers
{
    internal static class DcxHelper
    {
        public static DCX.CompressionInfo BuildCompressionInfo(string compressionText)
        {
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
                            compressionInfo = new DCX.DcxDfltCompressionInfo();
                            break;
                        case DCX.Type.DCX_KRAK:
                            compressionInfo = new DCX.DcxKrakCompressionInfo();
                            break;
                        case DCX.Type.DCX_ZSTD:
                            compressionInfo = new DCX.DcxZstdCompressionInfo();
                            break;
                        default:
                            throw new NotSupportedException($"{nameof(DCX.Type)} {compression} is not supported for: {nameof(BuildCompressionInfo)}");
                    }
                    break;
            }

            return compressionInfo;
        }
    }
}
