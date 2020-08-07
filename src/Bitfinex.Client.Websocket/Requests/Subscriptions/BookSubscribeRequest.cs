using Bitfinex.Client.Websocket.Utils;
using Bitfinex.Client.Websocket.Validations;

namespace Bitfinex.Client.Websocket.Requests.Subscriptions
{
    /// <summary>
    /// Subscribe for L2 order book data (grouped by price)
    /// </summary>
    public class BookSubscribeRequest : SubscribeRequestBase
    {
        /// <summary>
        /// Subscribe for L2 order book data (grouped by price)
        /// </summary>
        /// <param name="pair">Target symbol/pair (BTCUSD, ETHBTC, etc.)</param>
        /// <param name="precision">Level of price aggregation (P0, P1, P2, P3). The default is P0</param>
        /// <param name="frequency">Frequency of updates (F0, F1). F0=realtime / F1=2sec. The default is F0.</param>
        /// <param name="length">Number of price points ("25", "100") [default="25"]</param>
        public BookSubscribeRequest(
            string pair, 
            BitfinexPrecision precision = BitfinexPrecision.P0, 
            BitfinexFrequency frequency = BitfinexFrequency.Realtime, 
            string length = null)
        {
            BfxValidations.ValidateInput(pair, nameof(pair));

            Symbol = BitfinexSymbolUtils.FormatPairToSymbol(pair);
            Prec = precision.GetStringValue();
            Freq = frequency.GetStringValue();
            Len = string.IsNullOrWhiteSpace(length) ? "25" : length;
        }

        /// <summary>
        /// Websocket channel API
        /// </summary>
        public override string Channel => "book";

        /// <summary>
        /// Target symbol/pair
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Level of price aggregation (P0, P1, P2, P3). The default is P0
        /// </summary>
        public string Prec { get; }

        /// <summary>
        /// Frequency of updates (F0, F1).
        /// F0=realtime / F1=2sec.
        /// The default is F0.
        /// </summary>
        public string Freq { get; }

        /// <summary>
        /// Number of price points ("25", "100") [default="25"]
        /// </summary>
        public string Len { get; set; }
       
    }
}
