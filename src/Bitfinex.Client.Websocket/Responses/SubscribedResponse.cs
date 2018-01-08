using Bitfinex.Client.Websocket.Messages;

namespace Bitfinex.Client.Websocket.Responses
{
    public class SubscribedResponse : MessageBase
    {
        public string Channel { get; set; }
        public int ChanId { get; set; }
        public string Pair { get; set; }
    }
}
