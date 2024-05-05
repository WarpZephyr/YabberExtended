using System;

namespace YabberExtended
{
    /// <summary>
    /// Thrown when there is a parsing error.
    /// </summary>
    class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
    }
}
