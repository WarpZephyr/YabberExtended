using System.IO;
using System.Xml;
using YabberExtended.Extensions.Xml;

namespace YabberExtended.Extensions
{
    internal static class ArchiveExtensions
    {
        internal static string GetFilePathNameOrUseID(this XmlNode node, string directory, string id)
        {
            if (node.TryGetXmlValueOrContents("file", out string? filename) && !string.IsNullOrWhiteSpace(filename))
            {
                return YabberUtil.CorrectDirectorySeparator(Path.Combine(directory, filename));
            }

            return YabberUtil.CorrectDirectorySeparator(Path.Combine(directory, id));
        }
    }
}
