using System;

namespace YabberExtended
{
    class FriendlyException : Exception
    {
        public FriendlyException(string message) : base(message) { }
    }
}
