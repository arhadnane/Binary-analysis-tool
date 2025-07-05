// Conversion binaire → bytes, ASCII, Base64, Hex
using System;
using System.Text;

namespace BinaryAnalyzer.Core
{
    public static class BinaryParser
    {
        public static byte[] FromBinaryString(string binary)
        {
            if (string.IsNullOrWhiteSpace(binary))
                throw new ArgumentException("Input cannot be null or empty.");
            binary = binary.Replace(" ", "");
            if (binary.Length % 8 != 0)
                throw new ArgumentException("Binary string length must be a multiple of 8.");
            byte[] bytes = new byte[binary.Length / 8];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(binary.Substring(i * 8, 8), 2);
            }
            return bytes;
        }

        public static string ToAscii(byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }

        public static string ToBase64(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static string ToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}
