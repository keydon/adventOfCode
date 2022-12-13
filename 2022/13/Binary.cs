using System;
using System.Linq;

namespace aoc
{
    public static class BinaryExtensions
    {
        public static long FromBinaryToLong(this string binary)
        {
            return binary.Reverse().Select((c, i) => (c, i)).Where(k => k.c.ToInt() > 0).Select(k => (long)Math.Pow(2, k.i)).Sum();
        }
        public static long FromBinaryToLong(this char[] binary)
        {
            return binary.Reverse().Select((c, i) => (c, i)).Where(k => k.c.ToInt() > 0).Select(k => (long)Math.Pow(2, k.i)).Sum();
        }

        public static char[] ToBinary(this int number, int length)
        {
            return Convert.ToString(number, 2)
                        .PadLeft(length, '0')
                        .ToCharArray();
        }
    }
}