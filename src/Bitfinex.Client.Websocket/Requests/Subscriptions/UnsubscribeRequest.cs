using Bitfinex.Client.Websocket.Messages;

namespace Bitfinex.Client.Websocket.Requests.Subscriptions
{
    /// <summary>
    /// Unsubscribe from the channel.
    /// You need to subscribe to 'SubscriptionStream' in order to get correct channel id.
    /// </summary>
    public class UnsubscribeRequest: RequestBase
    {
        /// <summary>
        /// Unsubscribe from the channel request.
        /// Don't forget to set `ChanId`
        /// </summary>
        public UnsubscribeRequest()
        {
        }

        /// <summary>
        /// Unsubscribe from the channel.
        /// </summary>
        /// <param name="chanId">Target channel id</param>
        public UnsubscribeRequest(int chanId)
        {
            ChanId = chanId;
        }

        /// <inheritdoc />
        public override MessageType EventType => MessageType.Unsubscribe;

        /// <summary>
        /// Target channel id.
        /// You need to subscribe to 'SubscriptionStream' in order to get correct channel id.
        /// </summary>
        public int ChanId { get; set; }
    }
}
