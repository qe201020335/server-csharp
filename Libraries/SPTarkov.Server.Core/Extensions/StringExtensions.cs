using System.Text;

namespace SPTarkov.Server.Core.Extensions
{
    public static class StringExtensions
    {
        public static string Encode(this string value, EncodeType encode)
        {
            return encode switch
            {
                EncodeType.BASE64 => Convert.ToBase64String(Encoding.Default.GetBytes(value)),
                EncodeType.HEX => Convert.ToHexString(Encoding.Default.GetBytes(value)),
                EncodeType.ASCII => Encoding.ASCII.GetString(Encoding.Default.GetBytes(value)),
                EncodeType.UTF8 => Encoding.UTF8.GetString(Encoding.Default.GetBytes(value)),
                _ => throw new ArgumentOutOfRangeException(nameof(encode), encode, null),
            };
        }

        public static string Decode(this string value, EncodeType encode)
        {
            switch (encode)
            {
                case EncodeType.BASE64:
                    return Encoding.UTF8.GetString(Convert.FromBase64String(value));
                case EncodeType.HEX:
                    return Encoding.UTF8.GetString(Convert.FromHexString(value));
                case EncodeType.ASCII:
                    return Encoding.ASCII.GetString(Encoding.Default.GetBytes(value));
                case EncodeType.UTF8:
                    return Encoding.UTF8.GetString(Encoding.Default.GetBytes(value));
                default:
                    throw new ArgumentOutOfRangeException(nameof(encode), encode, null);
            }
        }

        public enum EncodeType
        {
            BASE64,
            HEX,
            ASCII,
            UTF8,
        }
    }
}
