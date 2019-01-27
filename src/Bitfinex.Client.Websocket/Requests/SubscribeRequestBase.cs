using Bitfinex.Client.Websocket.Messages;

namespace Bitfinex.Client.Websocket.Requests
{
    /// <summary>
    /// Base class for every subscription request
    /// </summary>
    public abstract class SubscribeRequestBase : RequestBase
    {
        /// <summary>
        /// Unique event type - subscribe
        /// </summary>
        public override MessageType EventType => MessageType.Subscribe;

        /// <summary>
        /// Unique channel name
        /// </summary>
        public abstract string Channel { get; }
    }
}
