using System;
using System.IO;

namespace YabberExtended
{
    /// <summary>
    /// Thrown to describe an error to a user in a more friendly way.
    /// </summary>
    class FriendlyException : Exception
    {
        public FriendlyException(string message) : base(message) { }

        public static void ThrowIfNotFile(string file)
        {
            if (!File.Exists(file))
            {
                throw new FriendlyException($"File not found: {file}");
            }
        }
    }
}
