using Bitfinex.Client.Websocket.Messages;

namespace Bitfinex.Client.Websocket.Responses
{
    /// <summary>
    /// Information about subscription
    /// </summary>
    public class SubscribedResponse : MessageBase
    {
        /// <summary>
        /// Channel name (trades, ticker. etc)
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// Unique channel id, you need to store this value in order to future unsubscription
        /// </summary>
        public int ChanId { get; set; }

        /// <summary>
        /// Target subscription pair
        /// </summary>
        public string Pair { get; set; }

        /// <summary>
        /// Target subscription symbol
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Target key
        /// </summary>
        public string Key { get; set; }
    }
}
