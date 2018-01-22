using Bitfinex.Client.Websocket.Utils;
using Bitfinex.Client.Websocket.Validations;

namespace Bitfinex.Client.Websocket.Requests
{
    public class BookSubscribeRequest: SubscribeRequestBase
    {
        public BookSubscribeRequest(
            string pair, 
            BitfinexPrecision precision = BitfinexPrecision.P0, 
            BitfinexFrequency frequency = BitfinexFrequency.Realtime, 
            string length = null)
        {
            BfxValidations.ValidateInput(pair, nameof(pair));

            Symbol = FormatPairToSymbol(pair);
            Prec = precision.GetStringValue();
            Freq = frequency.GetStringValue();
            Len = string.IsNullOrWhiteSpace(length) ? "25" : length;
        }

        public override string Channel => "book";
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
