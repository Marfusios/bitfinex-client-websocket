using Bitfinex.Client.Websocket.Messages;

namespace Bitfinex.Client.Websocket.Responses
{
    public class ErrorResponse : MessageBase
    {
        public string Code { get; set; }
        public string Msg { get; set; }
    }
}
