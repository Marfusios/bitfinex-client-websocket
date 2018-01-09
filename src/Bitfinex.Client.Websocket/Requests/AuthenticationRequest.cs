using Bitfinex.Client.Websocket.Messages;
using Bitfinex.Client.Websocket.Utils;
using Bitfinex.Client.Websocket.Validations;

namespace Bitfinex.Client.Websocket.Requests
{
    public class AuthenticationRequest : RequestBase
    {
        public AuthenticationRequest(string apiKey, string apiSecret)
        {
            BfxValidations.ValidateInput(apiKey, nameof(apiKey));
            BfxValidations.ValidateInput(apiSecret, nameof(apiSecret));

            ApiKey = apiKey;

            AuthNonce = BitfinexAuthentication.CreateAuthNonce();
            AuthPayload = BitfinexAuthentication.CreateAuthPayload(AuthNonce);

            AuthSig = BitfinexAuthentication.CreateSignature(AuthPayload, apiSecret);
        }

        public override MessageType EventType => MessageType.Auth;

        public string ApiKey { get; }
        public string AuthSig { get; }
        public long AuthNonce { get; }
        public string AuthPayload { get; }


        
    }
}
