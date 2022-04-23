using System;

namespace TerryBros.Utils
{
    public static partial class StringExtensions
    {
        public static string StringArray(this byte[] bytes) => Convert.ToHexString(bytes);

        public static byte[] ByteArray(this string str) => Convert.FromHexString(str);
    }
}
