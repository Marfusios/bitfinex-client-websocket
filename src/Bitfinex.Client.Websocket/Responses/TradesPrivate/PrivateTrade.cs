using System;
using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Responses.Configurations;
using Bitfinex.Client.Websocket.Responses.Orders;
using Bitfinex.Client.Websocket.Responses.Trades;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Bitfinex.Client.Websocket.Responses.TradesPrivate
{
    /// <summary>
    /// The order that causes the trade determines if it is a buy or a sell.
    /// </summary>
    [JsonConverter(typeof(PrivateTradeConverter))]
    public class PrivateTrade : ResponseBase
    {
        /// <summary>
        /// Trade id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Symbol (tBTCUSD, etc)
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Execution timestamp
        /// </summary>
        public DateTime MtsCreate { get; set; }

        /// <summary>
        /// Order id
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// How much was bought (positive) or sold (negative).
        /// </summary>
        public double ExecAmount { get; set; }

        /// <summary>
        /// Price at which the trade was executed
        /// </summary>
        public double ExecPrice { get; set; }

        /// <summary>
        /// Origin order type
        /// </summary>
        public OrderType OrderType { get; set; }

        /// <summary>
        /// Origin order target price
        /// </summary>
        public double OrderPrice { get; set; }

        /// <summary>
        /// True if maker order (post-only)
        /// </summary>
        public bool IsMaker { get; set; }

        /// <summary>
        /// Taken fee
        /// </summary>
        public double? Fee { get; set; }

        /// <summary>
        /// Taken fee currency
        /// </summary>
        public string FeeCurrency { get; set; }

        /// <summary>
        /// Type of the trade
        /// </summary>
        [JsonIgnore]
        public TradeType Type { get; set; }

        /// <summary>
        /// Target pair
        /// </summary>
        [JsonIgnore]
        public string Pair => BitfinexSymbolUtils.ExtractPair(Symbol);


        internal static void Handle(JToken token, ConfigurationState config, Subject<PrivateTrade> subject, TradeType type)
        {
            var data = token[2];
            if (data.Type != JTokenType.Array)
            {
                Log.Warning(L("Private trade info - Invalid message format, third param not array"));
                return;
            }

            var trade = data.ToObject<PrivateTrade>();
            trade.Type = type;
            SetGlobalData(trade, config, token, 2, true);

            subject.OnNext(trade);
        }

        private static string L(string msg)
        {
            return $"[BFX PRIVATE TRADE HANDLER] {msg}";
        }
    }
}
