using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace YabberExtended
{
    static class YBUtil
    {
        private static List<string> pathRoots = new List<string>
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

        private static readonly Regex DriveRx = new Regex(@"^(\w\:\\)(.+)$");
        private static readonly Regex SlashRx = new Regex(@"^(\\+)(.+)$");

        /// <summary>
        /// Removes common network path roots if present.
        /// </summary>
        public static string UnrootBNDPath(string path, out string root)
        {
            root = string.Empty;
            foreach (string pathRoot in pathRoots)
            {
                if (path.ToLower().StartsWith(pathRoot.ToLower()))
                {
                    root = path.Substring(0, pathRoot.Length);
                    path = path.Substring(pathRoot.Length);
                    break;
                }
            }

            Match drive = DriveRx.Match(path);
            if (drive.Success)
            {
                root = drive.Groups[1].Value;
                path = drive.Groups[2].Value;
            }

            return RemoveLeadingBackslashes(path, ref root);
        }

        private static string RemoveLeadingBackslashes(string path, ref string root)
        {
            Match slash = SlashRx.Match(path);
            if (slash.Success)
            {
                root += slash.Groups[1].Value;
                path = slash.Groups[2].Value;
            }
            return path;
        }

        public static string RemoveRootFromPath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                path = path.Substring(2);
            }

            return RemoveLeadingSlashes(path);
        }

        public static string RemoveLeadingSlashes(string path)
        {
            while (!string.IsNullOrEmpty(path) && (path[0] == '/' || path[0] == '\\'))
            {
                path = path.Substring(1);
            }
            return path;
        }

        public static void Backup(string path)
        {
            if (File.Exists(path) && !File.Exists(path + ".bak"))
                File.Move(path, path + ".bak");
        }

        public static string CorrectDirectorySeparator(string path)
        {
            return path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
        }

        #region Field Conversions

        public static sbyte FieldToSByte(string str, string name)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new FriendlyException($"{name} was missing or empty.");
            }

            if (!sbyte.TryParse(str, out sbyte result))
            {
                throw new FriendlyException($"{name} could not be parsed as a signed 8-bit number.");
            }

            return result;
        }

        public static sbyte FieldToSByte(string str, string name, sbyte defaultValue)
        {
            if (string.IsNullOrEmpty(str))
            {
                return defaultValue;
            }

            if (!sbyte.TryParse(str, out sbyte result))
            {
                throw new FriendlyException($"{name} could not be parsed as a signed 8-bit number.");
            }

            return result;
        }

        public static byte FieldToByte(string str, string name)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new FriendlyException($"{name} was missing or empty.");
            }

            if (!byte.TryParse(str, out byte result))
            {
                throw new FriendlyException($"{name} could not be parsed as an unsigned 8-bit number.");
            }

            return result;
        }

        public static byte FieldToByte(string str, string name, byte defaultValue)
        {
            if (string.IsNullOrEmpty(str))
            {
                return defaultValue;
            }

            if (!byte.TryParse(str, out byte result))
            {
                throw new FriendlyException($"{name} could not be parsed as an unsigned 8-bit number.");
            }

            return result;
        }

        public static short FieldToInt16(string str, string name)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new FriendlyException($"{name} was missing or empty.");
            }

            if (!short.TryParse(str, out short result))
            {
                throw new FriendlyException($"{name} could not be parsed as a signed 16-bit number.");
            }

            return result;
        }

        public static short FieldToInt16(string str, string name, short defaultValue)
        {
            if (string.IsNullOrEmpty(str))
            {
                return defaultValue;
            }

            if (!short.TryParse(str, out short result))
            {
                throw new FriendlyException($"{name} could not be parsed as a signed 16-bit number.");
            }

            return result;
        }

        public static ushort FieldToUInt16(string str, string name)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new FriendlyException($"{name} was missing or empty.");
            }

            if (!ushort.TryParse(str, out ushort result))
            {
                throw new FriendlyException($"{name} could not be parsed as an unsigned 16-bit number.");
            }

            return result;
        }

        public static ushort FieldToUInt16(string str, string name, ushort defaultValue)
        {
            if (string.IsNullOrEmpty(str))
            {
                return defaultValue;
            }

            if (!ushort.TryParse(str, out ushort result))
            {
                throw new FriendlyException($"{name} could not be parsed as an unsigned 16-bit number.");
            }

            return result;
        }

        public static int FieldToInt32(string str, string name)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new FriendlyException($"{name} was missing or empty.");
            }

            if (!int.TryParse(str, out int result))
            {
                throw new FriendlyException($"{name} could not be parsed as a signed 32-bit number.");
            }

            return result;
        }

        public static int FieldToInt32(string str, string name, int defaultValue)
        {
            if (string.IsNullOrEmpty(str))
            {
                return defaultValue;
            }

            if (!int.TryParse(str, out int result))
            {
                throw new FriendlyException($"{name} could not be parsed as a signed 32-bit number.");
            }

            return result;
        }

        public static uint FieldToUInt32(string str, string name)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new FriendlyException($"{name} was missing or empty.");
            }

            if (!uint.TryParse(str, out uint result))
            {
                throw new FriendlyException($"{name} could not be parsed as an unsigned 32-bit number.");
            }

            return result;
        }

        public static uint FieldToUInt32(string str, string name, uint defaultValue)
        {
            if (string.IsNullOrEmpty(str))
            {
                return defaultValue;
            }

            if (!uint.TryParse(str, out uint result))
            {
                throw new FriendlyException($"{name} could not be parsed as an unsigned 32-bit number.");
            }

            return result;
        }

        public static long FieldToInt64(string str, string name)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new FriendlyException($"{name} was missing or empty.");
            }

            if (!long.TryParse(str, out long result))
            {
                throw new FriendlyException($"{name} could not be parsed as a signed 64-bit number.");
            }

            return result;
        }

        public static long FieldToInt64(string str, string name, long defaultValue)
        {
            if (string.IsNullOrEmpty(str))
            {
                return defaultValue;
            }

            if (!long.TryParse(str, out long result))
            {
                throw new FriendlyException($"{name} could not be parsed as a signed 64-bit number.");
            }

            return result;
        }

        public static ulong FieldToUInt64(string str, string name)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new FriendlyException($"{name} was missing or empty.");
            }

            if (!ulong.TryParse(str, out ulong result))
            {
                throw new FriendlyException($"{name} could not be parsed as an unsigned 64-bit number.");
            }

            return result;
        }

        public static ulong FieldToUInt64(string str, string name, ulong defaultValue)
        {
            if (string.IsNullOrEmpty(str))
            {
                return defaultValue;
            }

            if (!ulong.TryParse(str, out ulong result))
            {
                throw new FriendlyException($"{name} could not be parsed as an unsigned 64-bit number.");
            }

            return result;
        }

        public static bool FieldToBool(string str, string name)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new FriendlyException($"{name} was missing or empty.");
            }

            if (!bool.TryParse(str, out bool result))
            {
                throw new FriendlyException($"{name} could not be parsed as a true or false boolean.");
            }

            return result;
        }

        public static bool FieldToBool(string str, string name, bool defaultValue)
        {
            if (string.IsNullOrEmpty(str))
            {
                return defaultValue;
            }

            if (!bool.TryParse(str, out bool result))
            {
                throw new FriendlyException($"{name} could not be parsed as a true or false boolean.");
            }

            return result;
        }

        public static string FieldToString(string str, string name)
        {
            if (string.IsNullOrEmpty(str))
            {
                throw new FriendlyException($"{name} was missing or empty.");
            }

            return str;
        }

        #endregion
    }
}
