using System.Security.Cryptography;
using System.Text;

namespace Bitfinex.Client.Websocket.Utils
{
    public static class BitfinexAuthentication
    {

        public static long CreateAuthNonce(long? time = null)
        {
            var timeSafe = time ?? BitfinexTime.NowMs();
            return timeSafe * 1000;
        }

        public static string CreateAuthPayload(long nonce)
        {
            return "AUTH" + nonce;
        }

        public static string CreateSignature(string payload, string apiSecret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(payload);
            var secretBytes = Encoding.UTF8.GetBytes(apiSecret);


            using (var hmacsha256 = new HMACSHA384(secretBytes))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(keyBytes);
                return ToLowerHex(hashmessage);
            }
        }

        private static string ToLowerHex(byte[] bytes)
        {
            var chars = new char[bytes.Length * 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                var value = bytes[i];
                chars[i * 2] = GetLowerHexChar(value >> 4);
                chars[i * 2 + 1] = GetLowerHexChar(value & 0xF);
            }

            return new string(chars);
        }

        private static char GetLowerHexChar(int value)
        {
            return (char)(value < 10 ? '0' + value : 'a' + value - 10);
        }
    }
}
