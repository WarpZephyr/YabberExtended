using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace YabberExtended
{
    static partial class YabberUtil
    {
        private static List<string> FromSoftwarePathRoots = new List<string>
        {
            // Demon's Souls
            @"N:\DemonsSoul\data\DVDROOT\",
            @"N:\DemonsSoul\data\",
            @"N:\DemonsSoul\",
            @"Z:\data\",

            // Ninja Blade
            @"I:\NinjaBlade\",

            // Dark Souls 1
            @"N:\FRPG\data\INTERROOT_win32\",
            @"N:\FRPG\data\INTERROOT_win64\",
            @"N:\FRPG\data\INTERROOT_x64\",
            @"N:\FRPG\data\INTERROOT\",
            @"N:\FRPG\data\",
            @"N:\FRPG\",

            // Dark Souls 2
            @"N:\FRPG2\data",
            @"N:\FRPG2\",
            @"N:\FRPG2_64\data\",
            @"N:\FRPG2_64\",

            // Dark Souls 3
            @"N:\FDP\data\INTERROOT_ps4\",
            @"N:\FDP\data\INTERROOT_win64\",
            @"N:\FDP\data\INTERROOT_xboxone\",
            @"N:\FDP\data\",
            @"N:\FDP\",

            // Bloodborne
            @"N:\SPRJ\data\DVDROOT_win64\",
            @"N:\SPRJ\data\INTERROOT_ps4\",
            @"N:\SPRJ\data\INTERROOT_ps4_havok\",
            @"N:\SPRJ\data\INTERROOT_win64\",
            @"N:\SPRJ\data\",
            @"N:\SPRJ\",

            // Sekiro
            @"N:\NTC\data\Target\INTERROOT_win64_havok\",
            @"N:\NTC\data\Target\INTERROOT_win64\",
            @"N:\NTC\data\Target\",
            @"N:\NTC\data\",
            @"N:\NTC\",
        };

        /// <summary>
        /// Removes common path roots if present.
        /// </summary>
        public static string SplitBinderRootFromPath(string path, out string root)
        {
            // Check if the path begins with a common FromSoftware path root.
            // If it does, split the root and add any proceeding slashes to it.
            // Then return the root and path.
            foreach (string pathRoot in FromSoftwarePathRoots)
            {
                if (path.StartsWith(pathRoot, StringComparison.CurrentCultureIgnoreCase))
                {
                    int start = pathRoot.Length;
                    while (start < path.Length)
                    {
                        // If the current index is not a slash:
                        if (path[start] != '\\' && path[start] != '/')
                        {
                            // Separate root from path and return.
                            root = path[..start];
                            return path[start..];
                        }

                        // If the current index was a slash continue.
                        start++;
                    }

                    // The path must only be a root, return root and an empty path.
                    root = path[..pathRoot.Length];
                    return string.Empty;
                }
            }

            // Split the drive root and any proceeding slashes if there was no common FromSoftware path.
            // Then return the root and path.
            SplitRootFromPath(path, out root, out path);
            return path;
        }

        public static void SplitRootFromPath(string inPath, out string root, out string outPath)
        {
            if (HasVolumeSeparator(inPath))
            {
                int start = 2;
                int length = inPath.Length;
                while (start < length)
                {
                    if (inPath[start] != '\\' && inPath[start] != '/')
                    {
                        break;
                    }

                    start++;
                }

                root = inPath[..start];
                outPath = inPath[start..];
            }
            else
            {
                root = string.Empty;
                outPath = inPath;
            }
        }

        public static string RemoveRootFromPath(string path)
        {
            if (HasVolumeSeparator(path))
            {
                int start = 2;
                int length = path.Length;
                while (start < length)
                {
                    if (path[start] != '\\' && path[start] != '/')
                    {
                        break;
                    }

                    start++;
                }

                return path[start..];
            }

            return path;
        }

        public static bool HasVolumeSeparator(string path)
            => path.Length >= 2 && (path[1] == ':' || path[1] == Path.VolumeSeparatorChar);

        public static string RemoveLeadingSlashes(string str)
            => str.TrimStart('\\', '/');

        public static string CorrectDirectorySeparator(string path)
            => path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);

        public static void BackupFile(string path)
        {
            if (File.Exists(path))
            {
                string backupPath = path + ".bak";
                if (!File.Exists(backupPath))
                {
                    File.Move(path, backupPath);
                }
            }
        }
    }
}
