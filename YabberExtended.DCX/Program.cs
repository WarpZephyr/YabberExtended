using SoulsFormats;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using ZstdNet;

namespace YabberExtended
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Console.WriteLine(
                    $"{assembly.GetName().Name} {assembly.GetName().Version}\n\n" +
                    "YabberExtended.DCX has no GUI.\n" +
                    "Drag and drop a DCX onto the exe to decompress it,\n" +
                    "or a decompressed file to recompress it.\n\n" +
                    "Press any key to exit."
                    );
                Console.ReadKey();
                return;
            }

            bool pause = false;

            foreach (string path in args)
            {
                try
                {
                    if (DCX.Is(path))
                    {
                        pause |= Decompress(path);
                    }
                    else
                    {
                        pause |= Compress(path);
                    }
                }
                catch (DllNotFoundException ex) when (ex.Message.Contains("oo2core_6_win64.dll"))
                {
                    Console.WriteLine("In order to decompress .dcx files from Sekiro, you must copy oo2core_6_win64.dll from Sekiro into Yabber's lib folder.");
                    pause = true;
                }
                catch (UnauthorizedAccessException)
                {
                    using (Process current = Process.GetCurrentProcess())
                    {
                        var admin = new Process();
                        admin.StartInfo = current.StartInfo;
                        admin.StartInfo.FileName = current.MainModule.FileName;
                        admin.StartInfo.Arguments = Environment.CommandLine.Replace($"\"{Environment.GetCommandLineArgs()[0]}\"", "");
                        admin.StartInfo.Verb = "runas";
                        admin.Start();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unhandled exception: {ex}");
                    pause = true;
                }

                Console.WriteLine();
            }

            if (pause)
            {
                Console.WriteLine("One or more errors were encountered and displayed above.\nPress any key to exit.");
                Console.ReadKey();
            }
        }

        private static bool Decompress(string sourceFile)
        {
            Console.WriteLine($"Decompressing DCX: {Path.GetFileName(sourceFile)}...");

            string sourceDir = Path.GetDirectoryName(sourceFile);
            string outPath;
            if (sourceFile.EndsWith(".dcx"))
                outPath = $"{sourceDir}\\{Path.GetFileNameWithoutExtension(sourceFile)}";
            else
                outPath = $"{sourceFile}.undcx";

            byte[] bytes = DCX.Decompress(sourceFile, out DCX.CompressionInfo compression);
            File.WriteAllBytes(outPath, bytes);

            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            XmlWriter xw = XmlWriter.Create($"{outPath}-yabber-dcx.xml", xws);

            xw.WriteStartElement("dcx");
            WriteCompressionInfo(xw, compression);
            xw.WriteEndElement();
            xw.Close();

            return false;
        }

        private static bool Compress(string path)
        {
            string xmlPath = $"{path}-yabber-dcx.xml";
            if (!File.Exists(xmlPath))
            {
                Console.WriteLine($"XML file not found: {xmlPath}");
                return true;
            }

            Console.WriteLine($"Compressing file: {Path.GetFileName(path)}...");
            XmlDocument xml = new XmlDocument();
            xml.Load(xmlPath);

            var compressionInfo = ReadCompressionInfo(xml.SelectSingleNode("dcx"));

            string outPath;
            if (path.EndsWith(".undcx"))
                outPath = path.Substring(0, path.Length - 6);
            else
                outPath = path + ".dcx";

            if (File.Exists(outPath) && !File.Exists(outPath + ".bak"))
                File.Move(outPath, outPath + ".bak");

            DCX.Compress(File.ReadAllBytes(path), compressionInfo, outPath);

            return false;
        }

        private static void WriteCompressionInfo(XmlWriter xw, DCX.CompressionInfo compression)
        {
            xw.WriteElementString("compression", compression.Type.ToString());
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

        private static DCX.CompressionInfo ReadCompressionInfo(XmlNode? node)
        {
            string compressionText = node?.SelectSingleNode("compression")?.InnerText ?? "None";

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
