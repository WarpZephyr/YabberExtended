using System.Text;

namespace YabberExtended.Helpers
{
    /// <summary>
    /// A helper containing encodings that are usually not easily accessed.
    /// </summary>
    public static class EncodingHelper
    {
        /// <summary>
        /// UTF-16 or Unicode encoding in little endian.
        /// </summary>
        public static readonly Encoding UTF16LE;

        /// <summary>
        /// UTF-16 or Unicode encoding in big endian.
        /// </summary>
        public static readonly Encoding UTF16BE;

        /// <summary>
        /// UTF-32 encoding in little endian.
        /// </summary>
        public static readonly Encoding UTF32LE;

        /// <summary>
        /// UTF-32 encoding in big endian.
        /// </summary>
        public static readonly Encoding UTF32BE;

        /// <summary>
        /// Japanese Shift-JIS encoding.
        /// </summary>
        public static readonly Encoding ShiftJIS;

        /// <summary>
        /// Japanese EUC-JP encoding.
        /// </summary>
        public static readonly Encoding EucJP;

        /// <summary>
        /// Chinese Simplified EUC-CN encoding.
        /// </summary>
        public static readonly Encoding EucCN;

        /// <summary>
        /// Korean EUC-KR encoding.
        /// </summary>
        public static readonly Encoding EucKR;

        /// <summary>
        /// Register the encodings.
        /// </summary>
        static EncodingHelper()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            UTF16LE = Encoding.Unicode;
            UTF16BE = Encoding.BigEndianUnicode;
            UTF32LE = Encoding.UTF32;
            UTF32BE = new UTF32Encoding(true, true);
            ShiftJIS = Encoding.GetEncoding("shift-jis");
            EucJP = Encoding.GetEncoding("euc-jp");
            EucCN = Encoding.GetEncoding("EUC-CN");
            EucKR = Encoding.GetEncoding("euc-kr");
        }

        /// <summary>
        /// Attempt to get the number of bytes per <see cref="char"/> for an <see cref="Encoding"/>.
        /// </summary>
        /// <param name="encoding">An <see cref="Encoding"/>.</param>
        /// <returns>The number of bytes the encoding is believed to use per <see cref="char"/>.</returns>
        internal static int GetByteCountPerChar(this Encoding encoding)
        {
            if (encoding is UnicodeEncoding)
            {
                return 2;
            }
            else if (encoding is UTF32Encoding)
            {
                return 4;
            }

            return 1;
        }
    }
}
