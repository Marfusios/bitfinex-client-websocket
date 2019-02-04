using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Client;
using Bitfinex.Client.Websocket.Logging;
using Bitfinex.Client.Websocket.Messages;
using Newtonsoft.Json;

using static Bitfinex.Client.Websocket.Client.BitfinexLogger;

namespace Bitfinex.Client.Websocket.Responses
{
    public class AuthenticationResponse : MessageBase
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger(); 

        public string Status { get; set; }
        public int ChanId { get; set; }
        public int UserId { get; set; }
        public string Code { get; set; }
        public object Caps { get; set; }

        /// <summary>
        /// Returns true if authentication succeed
        /// </summary>
        [JsonIgnore] 
        public bool IsAuthenticated => Status == "OK";


        internal static void Handle(string msg, Subject<AuthenticationResponse> subject)
        {
            var response = BitfinexSerialization.Deserialize<AuthenticationResponse>(msg);
;           if (!response.IsAuthenticated)
                Log.Warn(L("Authentication failed. Code: " + response.Code));
            subject.OnNext(response);
        }
    }
}
