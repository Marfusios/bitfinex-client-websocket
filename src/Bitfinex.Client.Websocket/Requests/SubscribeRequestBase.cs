using Bitfinex.Client.Websocket.Messages;

namespace Bitfinex.Client.Websocket.Requests
{
    public abstract class SubscribeRequestBase : RequestBase
    {
        public override MessageType EventType => MessageType.Subscribe;

        public abstract string Channel { get; }
    }
}
