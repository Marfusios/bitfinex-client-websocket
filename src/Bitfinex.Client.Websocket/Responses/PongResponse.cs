using System;
using Bitfinex.Client.Websocket.Json;
using Bitfinex.Client.Websocket.Messages;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Responses
{
    public class PongResponse : MessageBase
    {
        public int Cid { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Ts { get; set; }
    }
}
