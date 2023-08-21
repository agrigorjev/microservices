using System;
using System.Text;

namespace Mandara.Entities.Extensions
{
    public static class StringExtensions
    {
        public static bool EqualTrimmed(this string instance, string string2)
        {
            if (instance == null)
                instance = "";

            if (string2 == null)
                string2 = "";

            return instance.Trim() == string2.Trim();
        }

        public static bool EqualTrimmed(this string instance, string string2, StringComparison stringComparison)
        {
            if (instance == null)
                instance = "";

            if (string2 == null)
                string2 = "";

            return instance.Trim().Equals(string2.Trim(), stringComparison);
        }

        public static string ConvertToAscii(this string instance)
        {
            if (instance == null)
                return null;

            string sOut = Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(instance));

            if (sOut.Equals(instance))
                return instance;

            // Create two different encodings.
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;

            // Convert the string into a byte array.
            byte[] unicodeBytes = unicode.GetBytes(instance);

            // Perform the conversion from one encoding to the other.
            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);

            // Convert the new byte[] into a char[] and then into a string.
            char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            string asciiString = new string(asciiChars);

            return asciiString;
        }

        // string extension method ToUpperFirstLetter
        public static string ToUpperFirstLetter(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            // convert to char array of the string
            char[] letters = source.ToCharArray();
            // upper case the first char
            letters[0] = char.ToUpper(letters[0]);
            // return the array made of the new char array
            return new string(letters);
        }

        public static string Reverse(this string source)
        {
            StringBuilder output = new StringBuilder();

            for (int i = source.Length - 1; i >= 0; --i)
            {
                output.Append(source[i]);
            }

            return output.ToString();
        }
    }
}