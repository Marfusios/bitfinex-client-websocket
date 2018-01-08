using System.Security.Cryptography;
using System.Text;
using Bitfinex.Client.Websocket.Messages;
using Bitfinex.Client.Websocket.Utils;
using Bitfinex.Client.Websocket.Validations;

namespace Bitfinex.Client.Websocket.Requests
{
    public class AuthenticationRequest : RequestBase
    {
        private readonly string _apiSecret;

        public AuthenticationRequest(string apiKey, string apiSecret)
        {
            BfxValidations.ValidateInput(apiKey, nameof(apiKey));
            BfxValidations.ValidateInput(apiSecret, nameof(apiSecret));

            ApiKey = apiKey;
            _apiSecret = apiSecret;

            AuthNonce = BitfinexTime.NowMs() * 1000;
            AuthPayload = "AUTH" + AuthNonce;

            AuthSig = CreateSignature();
        }

        public override MessageType EventType => MessageType.Auth;

        public string ApiKey { get; }
        public string AuthSig { get; }
        public long AuthNonce { get; }
        public string AuthPayload { get; }


        private string CreateSignature()
        {
            var keyBytes = Encoding.UTF8.GetBytes(AuthPayload);
            var secretBytes = Encoding.UTF8.GetBytes(_apiSecret);


            string ByteToString(byte[] buff)
            {
                var builder = new StringBuilder();

                for (var i = 0; i < buff.Length; i++)
                {
                    builder.Append(buff[i].ToString("X2")); // hex format
                }
                return builder.ToString();
            }

            using (var hmacsha256 = new HMACSHA384(secretBytes))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(keyBytes);
                return ByteToString(hashmessage).ToLower();
            }
        }
    }
}
