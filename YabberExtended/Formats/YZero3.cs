using System;
using System.IO;
using SoulsFormats;

namespace YabberExtended
{
    static class YZero3
    {
        public static void Unpack(this Zero3 z3, string targetDir)
        {
            foreach (Zero3.File file in z3.Files)
            {
                string name = file.Name.Replace('/', '\\');
                if (name.Length > 2 && name[1] == ':')
                {
                    name = name[2..];
                }

                string outPath = Path.Combine(targetDir, name);
                Directory.CreateDirectory(Path.GetDirectoryName(outPath) ?? throw new Exception($"Could not get folder name for: {outPath}"));
                File.WriteAllBytes(outPath, file.Bytes);
            }
        }
    }
}
