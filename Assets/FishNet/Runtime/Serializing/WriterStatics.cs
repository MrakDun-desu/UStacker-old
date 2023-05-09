using System;
using System.Text;
using FishNet.Documenting;

namespace FishNet.Serializing
{
    /// <summary>
    ///     Writes data to a buffer.
    /// </summary>
    [APIExclude]
    internal class WriterStatics
    {
        /// <summary>
        ///     Gets the string buffer ensuring proper length, and outputs size in bytes of string.
        /// </summary>
        public static byte[] GetStringBuffer(string str, out int size)
        {
            var strLength = str.Length;
            var valueMaxBytes = _encoding.GetMaxByteCount(strLength);
            if (valueMaxBytes >= _stringBuffer.Length)
            {
                var nextSize = _stringBuffer.Length * 2 + valueMaxBytes;
                Array.Resize(ref _stringBuffer, nextSize);
            }

            size = _encoding.GetBytes(str, 0, strLength, _stringBuffer, 0);
            return _stringBuffer;
        }

        /// <summary>
        ///     Ensures the string buffer is of a minimum length and returns the buffer.
        /// </summary>
        public static byte[] GetStringBuffer(string str)
        {
            var valueMaxBytes = _encoding.GetMaxByteCount(str.Length);
            if (valueMaxBytes >= _stringBuffer.Length)
            {
                var nextSize = _stringBuffer.Length * 2 + valueMaxBytes;
                Array.Resize(ref _stringBuffer, nextSize);
            }

            return _stringBuffer;
        }
        /* Since serializing occurs on the main thread this value may
        * be shared among all writers. //multithread
        */

        #region Private.

        /// <summary>
        ///     Encoder for strings.
        /// </summary>
        private static readonly UTF8Encoding _encoding = new(false, true);

        /// <summary>
        ///     StringBuffer to use with encoding.
        /// </summary>
        private static byte[] _stringBuffer = new byte[64];

        #endregion
    }
}