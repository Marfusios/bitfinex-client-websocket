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

        /// <summary>
        /// Level of price aggregation one of: P0, P1, P2, P3, R0 (raw)
        /// </summary>
        public string Prec { get; set; }

        /// <summary>
        /// Number of orders/levels returned by API ("1", "25", "100")
        /// </summary>
        public string Len { get; set; }
    }
}
