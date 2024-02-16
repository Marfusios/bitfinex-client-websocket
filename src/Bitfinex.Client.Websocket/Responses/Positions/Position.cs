using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Responses.Configurations;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Positions
{
    /// <summary>
    /// Info about taken position
    /// </summary>
    [DebuggerDisplay("Position: {Pair} - {BasePrice} - {Amount}")]
    [JsonConverter(typeof(PositionConverter))]
    public class Position : ResponseBase
    {
        /// <summary>
        /// Pair (tBTCUSD, etc). 
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Status (ACTIVE, CLOSED).
        /// </summary>
        public PositionStatus Status { get; set; }

        /// <summary>
        /// Size of the position. Positive values means a long position, negative values means a short position. 0 means position closed.
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// The price at which you entered your position.
        /// </summary>
        public double BasePrice { get; set; }

        /// <summary>
        /// The amount of funding being used for this position.
        /// </summary>
        public double MarginFunding { get; set; }

        /// <summary>
        /// Margin funding type (daily or term)
        /// </summary>
        public MarginFundingType MarginFundingType { get; set; }

        /// <summary>
        /// Profit & Loss
        /// </summary>
        public double? ProfitLoss { get; set; }

        /// <summary>
        /// Profit & Loss percentage
        /// </summary>
        public double? ProfitLossPercentage { get; set; }

        /// <summary>
        /// Liquidation price
        /// Could equal to "null" meaning that the new calculated value is not yet available.
        /// In order to receive those values the user have to actively request for it with a "calc" message.
        /// </summary>
        public double? LiquidationPrice { get; set; }


        /// <summary>
        /// Removes trailing 'f' or 't' and returns raw pair
        /// </summary>
        public string Pair => BitfinexSymbolUtils.ExtractPair(Symbol);

        /// <summary>
        /// Base symbol (first position: BTC in BTCUSD)
        /// </summary>
        public string BaseSymbol => BitfinexSymbolUtils.ExtractBaseSymbol(Pair);

        /// <summary>
        /// Quote symbol (second position: USD in BTCUSD)
        /// </summary>
        public string QuoteSymbol => BitfinexSymbolUtils.ExtractQuoteSymbol(Pair);



        internal static void Handle(JToken token, ConfigurationState config, Subject<Position[]> subject)
        {
            var data = token[2];
            if (data?.Type != JTokenType.Array)
            {
                return;
            }

            var parsed = data.ToObject<Position[]>() ?? Array.Empty<Position>();
            foreach (var position in parsed)
            {
                SetGlobalData(position, config, token, 2, true);
            }
            subject.OnNext(parsed);
        }

        internal static void Handle(JToken token, ConfigurationState config, Subject<Position> subject)
        {
            var data = token[2];
            if (data?.Type != JTokenType.Array)
            {
                return;
            }

            var position = data.ToObject<Position>();
            if (position != null)
            {
                SetGlobalData(position, config, token, 2, true);
                subject.OnNext(position);
            }
        }
    }
}
