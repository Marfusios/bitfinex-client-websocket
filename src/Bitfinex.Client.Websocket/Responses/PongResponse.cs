using Bitfinex.Client.Websocket.Messages;

namespace Bitfinex.Client.Websocket.Responses
{
    public class PongResponse : MessageBase
    {
        public int Cid { get; set; }
        public string Ts { get; set; }
    }
}
