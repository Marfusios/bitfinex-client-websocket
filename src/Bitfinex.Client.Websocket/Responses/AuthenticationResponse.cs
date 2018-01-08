using Bitfinex.Client.Websocket.Messages;

namespace Bitfinex.Client.Websocket.Responses
{
    public class AuthenticationResponse : MessageBase
    {
        public string Status { get; set; }
        public int ChanId { get; set; }
        public int UserId { get; set; }
        public string Code { get; set; }
        public object Caps { get; set; }
    }
}
