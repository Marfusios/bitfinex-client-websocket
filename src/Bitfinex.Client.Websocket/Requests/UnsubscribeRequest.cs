using Bitfinex.Client.Websocket.Messages;

namespace Bitfinex.Client.Websocket.Requests
{
    /// <summary>
    /// Unsubscribe from the channel.
    /// You need to subscribe to 'SubscriptionStream' in order to get correct channel id.
    /// </summary>
    public class UnsubscribeRequest: RequestBase
    {
        /// <inheritdoc />
        public override MessageType EventType => MessageType.Unsubscribe;

        /// <summary>
        /// Target channel id.
        /// You need to subscribe to 'SubscriptionStream' in order to get correct channel id.
        /// </summary>
        public int ChanId { get; set; }
    }
}
