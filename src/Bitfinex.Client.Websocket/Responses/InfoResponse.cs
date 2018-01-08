using Bitfinex.Client.Websocket.Messages;

namespace Bitfinex.Client.Websocket.Responses
{
    public class InfoResponse : MessageBase
    {
        public string Version { get; set; }
        public string Code { get; set; }
        public string Msg { get; set; }
    }
}
