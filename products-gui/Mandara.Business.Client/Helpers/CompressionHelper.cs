using System;
using System.Linq;

namespace Mandara.Business.Helpers
{
    public static class CompressionHelper
    {
        private static byte[] GZipHeaderBytes = { 0x1f, 0x8b, 8};

        public static bool IsPossiblyGZippedBytes(this byte[] a)
        {
            if (a.Length < 3)
            {
                return false;
            }

            byte[] header = a.SubArray(0, 3);

            return header.SequenceEqual(GZipHeaderBytes);
        }

        private static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);

            return result;
        }
    }
}