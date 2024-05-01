using SoulsFormats;
using SoulsFormats.AC4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

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
                    "YabberExtended has no GUI.\n" +
                    "Drag and drop a file onto the exe to unpack it,\n" +
                    "or an unpacked folder to repack it.\n\n" +
                    "DCX files will be transparently decompressed and recompressed;\n" +
                    "If you need to decompress or recompress an unsupported format,\n" +
                    "use YabberExtended.DCX instead.\n\n" +
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
                    int maxProgress = Console.WindowWidth - 1;
                    int lastProgress = 0;
                    void report(float value)
                    {
                        int nextProgress = (int)Math.Ceiling(value * maxProgress);
                        if (nextProgress > lastProgress)
                        {
                            for (int i = lastProgress; i < nextProgress; i++)
                            {
                                if (i == 0)
                                    Console.Write('[');
                                else if (i == maxProgress - 1)
                                    Console.Write(']');
                                else
                                    Console.Write('=');
                            }
                            lastProgress = nextProgress;
                        }
                    }
                    IProgress<float> progress = new Progress<float>(report);

                    if (Directory.Exists(path))
                    {
                        pause |= RepackDir(path, progress);

                    }
                    else if (File.Exists(path))
                    {
                        pause |= UnpackFile(path, progress);
                    }
                    else
                    {
                        Console.WriteLine($"File or directory not found: {path}");
                        pause = true;
                    }

                    if (lastProgress > 0)
                    {
                        progress.Report(1);
                        Console.WriteLine();
                    }
                }
                catch (DllNotFoundException ex) when (ex.Message.Contains("oo2core_6_win64.dll"))
                {
                    Console.WriteLine("In order to decompress .dcx files from Sekiro, you must copy oo2core_6_win64.dll from Sekiro into YabberExtended's lib folder.");
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
                catch (FriendlyException ex)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Error: {ex.Message}");
                    pause = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
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

        private static bool UnpackFile(string sourceFile, IProgress<float> progress)
        {
            string sourceDir = Path.GetDirectoryName(sourceFile);
            string filename = Path.GetFileName(sourceFile);
            string filenameLower = filename.ToLowerInvariant();
            string targetDir = Path.Combine(sourceDir, filename.Replace('.', '-'));

            if (File.Exists(targetDir))
                targetDir += "-ybr";

            if (DCX.Is(sourceFile))
            {
                Console.WriteLine($"Decompressing DCX: {filename}...");
                byte[] bytes = DCX.Decompress(sourceFile, out DCX.Type compression);
                if (BND3.Is(bytes))
                {
                    Console.WriteLine($"Unpacking BND3: {filename}...");
                    using (var bnd = new BND3Reader(bytes))
                    {
                        bnd.Compression = compression;
                        bnd.Unpack(filename, targetDir, progress);
                    }
                }
                else if (BND4.Is(bytes))
                {
                    Console.WriteLine($"Unpacking BND4: {filename}...");
                    using (var bnd = new BND4Reader(bytes))
                    {
                        bnd.Compression = compression;
                        bnd.Unpack(filename, targetDir, progress);
                    }
                }
                else if (FFXDLSE.Is(bytes))
                {
                    Console.WriteLine($"Unpacking FFX: {filename}...");
                    var ffx = FFXDLSE.Read(bytes);
                    ffx.Compression = compression;
                    ffx.Unpack(sourceFile);
                }
                else if (sourceFile.EndsWith(".fmg.dcx"))
                {
                    Console.WriteLine($"Unpacking FMG: {filename}...");
                    FMG fmg = FMG.Read(bytes);
                    fmg.Compression = compression;
                    fmg.Unpack(sourceFile);
                }
                else if (GPARAM.Is(bytes))
                {
                    Console.WriteLine($"Unpacking GPARAM: {filename}...");
                    GPARAM gparam = GPARAM.Read(bytes);
                    gparam.Compression = compression;
                    gparam.Unpack(sourceFile);
                }
                else if (TPF.Is(bytes))
                {
                    Console.WriteLine($"Unpacking TPF: {filename}...");
                    TPF tpf = TPF.Read(bytes);
                    tpf.Compression = compression;
                    tpf.Unpack(filename, targetDir, progress);
                }
                else
                {
                    Console.WriteLine($"File format not recognized: {filename}");
                    return true;
                }
            }
            else
            {
                if (BND3.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking BND3: {filename}...");
                    using (var bnd = new BND3Reader(sourceFile))
                    {
                        bnd.Unpack(filename, targetDir, progress);
                    }
                }
                else if (BND4.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking BND4: {filename}...");
                    using (var bnd = new BND4Reader(sourceFile))
                    {
                        bnd.Unpack(filename, targetDir, progress);
                    }
                }
                else if (BND2.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking BND2: {filename}...");
                    using (var bnd = new BND2Reader(sourceFile))
                    {
                        bnd.Unpack(filename, targetDir, progress);
                    }
                }
                else if (SoulsFormats.AC3SL.BND.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking AC3SL BND: {filename}...");
                    var bnd = SoulsFormats.AC3SL.BND.Read(sourceFile);
                    bnd.Unpack(filename, targetDir, progress);
                }
                else if (SoulsFormats.Kuon.DVDBND.Is(sourceFile))
                {
                    try
                    {
                        var dvdBndKuon = SoulsFormats.Kuon.DVDBND.Read(sourceFile);
                        Console.WriteLine($"Unpacking Kuon DVDBND: {filename}...");
                        dvdBndKuon.Unpack(filename, targetDir, progress);
                    }
                    catch
                    {
                        try
                        {
                            var bndKuon = SoulsFormats.Kuon.BND.Read(sourceFile);
                            Console.WriteLine($"Unpacking Kuon BND: {filename}...");
                            bndKuon.Unpack(filename, targetDir, progress);
                        }
                        catch
                        {
                            try
                            {
                                var bndACE3 = SoulsFormats.ACE3.BND.Read(sourceFile);
                                Console.WriteLine($"Unpacking ACE3 BND: {filename}...");
                                bndACE3.Unpack(filename, targetDir, progress);
                            }
                            catch { }
                        }
                    }
                }
                else if (BXF3.IsHeader(sourceFile))
                {
                    string bdtExtension = Path.GetExtension(filename).Replace("bhd", "bdt");
                    string bdtFilename = $"{Path.GetFileNameWithoutExtension(filename)}{bdtExtension}";
                    string bdtPath = Path.Combine(sourceDir, bdtFilename);
                    if (File.Exists(bdtPath))
                    {
                        Console.WriteLine($"Unpacking BXF3: {filename}...");
                        using (var bxf = new BXF3Reader(sourceFile, bdtPath))
                        {
                            bxf.Unpack(filename, bdtFilename, targetDir, progress);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"BDT not found for BHD: {filename}");
                        return true;
                    }
                }
                else if (BXF4.IsHeader(sourceFile))
                {
                    string bdtExtension = Path.GetExtension(filename).Replace("bhd", "bdt");
                    string bdtFilename = $"{Path.GetFileNameWithoutExtension(filename)}{bdtExtension}";
                    string bdtPath = Path.Combine(sourceDir, bdtFilename);
                    if (File.Exists(bdtPath))
                    {
                        Console.WriteLine($"Unpacking BXF4: {filename}...");
                        using (var bxf = new BXF4Reader(sourceFile, bdtPath))
                        {
                            bxf.Unpack(filename, bdtFilename, targetDir, progress);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"BDT not found for BHD: {filename}");
                        return true;
                    }
                }
                else if (LDMU.Is(sourceFile))
                {
                    string bndFilename = $"{Path.GetFileNameWithoutExtension(filename)}.bnd";
                    string bndPath = Path.Combine(sourceDir, bndFilename);
                    if (File.Exists(bndPath))
                    {
                        Console.WriteLine($"Unpacking LDMU: {filename}...");
                        var ldmu = LDMU.Read(sourceFile, bndPath);
                        ldmu.Unpack(filename, bndFilename, targetDir, progress);
                    }
                    else
                    {
                        Console.WriteLine($"BND not found for BHD: {filename}");
                        return true;
                    }
                }
                else if (FFXDLSE.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking FFX: {filename}...");
                    var ffx = FFXDLSE.Read(sourceFile);
                    ffx.Unpack(sourceFile);
                }
                else if (TPF.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking TPF: {filename}...");
                    TPF tpf = TPF.Read(sourceFile);
                    tpf.Unpack(filename, targetDir, progress);
                }
                else if (Zero3.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking 000: {filename}...");
                    Zero3 z3 = Zero3.Read(sourceFile);
                    z3.Unpack(targetDir);
                }
                else if (ANC.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking ANC: {filename}...");
                    ANC anc = ANC.Read(sourceFile);
                    anc.Unpack(filename, targetDir);
                }
                else if (MQB.Is(sourceFile))
                {
                    Console.WriteLine($"Converting MQB: {filename}...");
                    MQB mqb = MQB.Read(sourceFile);
                    mqb.Unpack(filename, sourceDir, progress);
                }
                else if (sourceFile.EndsWith(".ffx.xml") || sourceFile.EndsWith(".ffx.dcx.xml"))
                {
                    Console.WriteLine($"Repacking FFX: {filename}...");
                    YFFX.Repack(sourceFile);
                }
                else if (sourceFile.EndsWith(".fmg"))
                {
                    Console.WriteLine($"Unpacking FMG: {filename}...");
                    FMG fmg = FMG.Read(sourceFile);
                    fmg.Unpack(sourceFile);
                }
                else if (sourceFile.EndsWith(".fmg.xml") || sourceFile.EndsWith(".fmg.dcx.xml"))
                {
                    Console.WriteLine($"Repacking FMG: {filename}...");
                    YFMG.Repack(sourceFile);
                }
                else if (GPARAM.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking GPARAM: {filename}...");
                    GPARAM gparam = GPARAM.Read(sourceFile);
                    gparam.Unpack(sourceFile);
                }
                else if (sourceFile.EndsWith(".gparam.xml") || sourceFile.EndsWith(".gparam.dcx.xml")
                    || sourceFile.EndsWith(".fltparam.xml") || sourceFile.EndsWith(".fltparam.dcx.xml"))
                {
                    Console.WriteLine($"Repacking GPARAM: {filename}...");
                    YGPARAM.Repack(sourceFile);
                }
                else if (sourceFile.EndsWith(".luagnl"))
                {
                    Console.WriteLine($"Unpacking LUAGNL: {filename}...");
                    LUAGNL gnl = LUAGNL.Read(sourceFile);
                    gnl.Unpack(sourceFile);
                }
                else if (sourceFile.EndsWith(".luagnl.xml"))
                {
                    Console.WriteLine($"Repacking LUAGNL: {filename}...");
                    YLUAGNL.Repack(sourceFile);
                }
                else if (LUAINFO.Is(sourceFile))
                {
                    Console.WriteLine($"Unpacking LUAINFO: {filename}...");
                    LUAINFO info = LUAINFO.Read(sourceFile);
                    info.Unpack(sourceFile);
                }
                else if (sourceFile.EndsWith(".luainfo.xml"))
                {
                    Console.WriteLine($"Repacking LUAINFO: {filename}...");
                    YLUAINFO.Repack(sourceFile);
                }
                else if (sourceFile.EndsWith(".mqb.xml"))
                {
                    Console.WriteLine($"Converting XML to MQB: {filename}...");
                    YMQB.Repack(sourceFile);
                }
                else if (filename.EndsWith("DATA.BIN", StringComparison.InvariantCultureIgnoreCase))
                {
                    int entryCount;
                    bool compressed;
                    switch (filename)
                    {
                        case "ERDATA.BIN":
                        case "AC2DATA.BIN":
                            entryCount = 4096;
                            compressed = false;
                            break;
                        case "AC25DATA.BIN":
                        case "AC3DATA.BIN":
                            entryCount = 8192;
                            compressed = false;
                            break;
                        case "ac3data.bin":
                            entryCount = 8192;
                            compressed = true;
                            break;
                        default:
                            entryCount = -1;
                            compressed = false;
                            break;
                    }

                    if (entryCount != -1)
                    {
                        Console.WriteLine($"Unpacking FSDATA: {filename}...");
                        var data = FSDATA.Read(sourceFile, entryCount, compressed);
                        data.Unpack(filename, targetDir, progress);
                    }
                }
                else if (filenameLower == "acparts.bin" || filenameLower == "enemyparts.bin" || filenameLower == "stabilizer.bin")
                {
                    AcParts4.AcParts4Version version = PromptVersion<AcParts4.AcParts4Version>("AcParts4");
                    Console.WriteLine("Unpacking AcParts4...");
                    var acparts = AcParts4.Read(sourceFile, version);
                    acparts.Unpack(filename, targetDir);
                }
                else if (filename == "acvparts.bin")
                {
                    Console.WriteLine("Armored Core 5thgen AcParts is not supported.");
                    return true;
                }
                else
                {
                    Console.WriteLine($"File format not recognized: {filename}");
                    return true;
                }
            }
            return false;
        }

        private static bool RepackDir(string sourceDir, IProgress<float> progress)
        {
            string sourceName = new DirectoryInfo(sourceDir).Name;
            string targetDir = new DirectoryInfo(sourceDir).Parent.FullName;
            if (File.Exists(Path.Combine(sourceDir, "_yabber-bnd2.xml")))
            {
                Console.WriteLine($"Repacking BND2: {sourceName}...");
                YBND2.Repack(sourceDir, targetDir);
            }
            else if (File.Exists(Path.Combine(sourceDir, "_yabber-bnd3.xml")))
            {
                Console.WriteLine($"Repacking BND3: {sourceName}...");
                YBND3.Repack(sourceDir, targetDir);
            }
            else if (File.Exists(Path.Combine(sourceDir, "_yabber-bnd4.xml")))
            {
                Console.WriteLine($"Repacking BND4: {sourceName}...");
                YBND4.Repack(sourceDir, targetDir);
            }
            else if (File.Exists(Path.Combine(sourceDir, "_yabber-bxf3.xml")))
            {
                Console.WriteLine($"Repacking BXF3: {sourceName}...");
                YBXF3.Repack(sourceDir, targetDir);
            }
            else if (File.Exists(Path.Combine(sourceDir, "_yabber-bxf4.xml")))
            {
                Console.WriteLine($"Repacking BXF4: {sourceName}...");
                YBXF4.Repack(sourceDir, targetDir);
            }
            else if (File.Exists(Path.Combine(sourceDir, "_yabber-ldmu.xml")))
            {
                Console.WriteLine($"Repacking LDMU: {sourceName}...");
                YLDMU.Repack(sourceDir, targetDir);
            }
            else if (File.Exists(Path.Combine(sourceDir, "_yabber-fsdata.xml")))
            {
                Console.WriteLine($"Repacking FSDATA: {sourceName}...");
                YFSDATA.Repack(sourceDir, targetDir);
            }
            else if (File.Exists(Path.Combine(sourceDir, "_yabber-tpf.xml")))
            {
                Console.WriteLine($"Repacking TPF: {sourceName}...");
                YTPF.Repack(sourceDir, targetDir);
            }
            else if (File.Exists(Path.Combine(sourceDir, "_yabber-anc.xml")))
            {
                Console.WriteLine($"Repacking ANC: {sourceName}...");
                YANC.Repack(sourceDir, targetDir);
            }
            else if (File.Exists(Path.Combine(sourceDir, "_yabber-bnd_ac3sl.xml")))
            {
                Console.WriteLine($"Repacking AC3SL BND: {sourceName}...");
                YBNDAC3SL.Repack(sourceDir, targetDir);
            }
            else if (File.Exists(Path.Combine(sourceDir, "_yabber-bnd_kuon.xml")))
            {
                Console.WriteLine($"Repacking Kuon BND: {sourceName}...");
                YBNDKUON.Repack(sourceDir, targetDir);
            }
            else if (File.Exists(Path.Combine(sourceDir, "_yabber-bnd_ace3.xml")))
            {
                Console.WriteLine($"Repacking ACE3 BND: {sourceName}...");
                YBNDACE3.Repack(sourceDir, targetDir);
            }
            else
            {
                Console.WriteLine($"Yabber XML not found in: {sourceName}");
                return true;
            }
            return false;
        }

        private static TEnum PromptVersion<TEnum>(string format) where TEnum : struct
        {
            var names = Enum.GetNames(typeof(TEnum));
            string strVersion = PromptVersion(format, names);
            if (!Enum.TryParse(strVersion, out TEnum version))
            {
                throw new InvalidDataException($"Selected version not present in enum: {version}");
            }

            return version;
        }

        private static string PromptVersion(string format, params string[] options)
        {
            Console.WriteLine($"Please choose a version for {format}:");
            Console.WriteLine($"Options:");
            for (int i = 0; i < options.Length; i++)
            {
                int num = i + 1;
                Console.WriteLine($"{num}: {options[i]}");
            }

            string version = Console.ReadLine().Trim();
            if (Array.IndexOf(options, version) == -1)
            {
                if (!int.TryParse(version, out int chosenNum))
                {
                    throw new FriendlyException("That option is not supported.");
                }

                int index = chosenNum - 1;
                if (index >= options.Length)
                {
                    throw new FriendlyException("That option is not supported.");
                }

                version = options[index];
            }

            return version;
        }
    }
}
