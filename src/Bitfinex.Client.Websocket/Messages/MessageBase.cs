namespace Bitfinex.Client.Websocket.Messages
{
    public class MessageBase
    {
        public virtual MessageType Event { get; set; }
    }
}
