using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Bitfinex.Client.Websocket.Responses.Orders
{
    /// <summary>
    /// Info about placed order
    /// </summary>
    [DebuggerDisplay("Order: {Id}/{Cid} - {Symbol} - {Amount}")]
    [JsonConverter(typeof(OrderConverter))]
    public class Order
    {
        /// <summary>
        /// Order ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Group ID
        /// </summary>
        public long? Gid { get; set; }

        /// <summary>
        /// Client Order ID
        /// </summary>
        public long Cid { get; set; }

        /// <summary>
        /// Pair (tBTCUSD, …)
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Millisecond timestamp of creation
        /// </summary>
        public DateTime? MtsCreate { get; set; }

        /// <summary>
        /// Millisecond timestamp of update
        /// </summary>
        public DateTime? MtsUpdate { get; set; }

        /// <summary>
        /// Positive means buy, negative means sell.
        /// </summary>
        public double? Amount { get; set; }

        /// <summary>
        /// Original amount
        /// </summary>
        public double? AmountOrig { get; set; }

        /// <summary>
        /// The type of the order
        /// </summary>
        public OrderType Type { get; set; }

        /// <summary>
        /// Previous order type
        /// </summary>
        public OrderType TypePrev { get; set; }

        /// <summary>
        /// Millisecond timestamp of Time-In-Force: automatic order cancellation
        /// </summary>
        public DateTime? MtsTiff { get; set; }

        /// <summary>
        /// Hidden | Close | Post Only | Reduce Only | No Var Rates | OCO
        /// </summary>
        public int? Flags { get; set; }

        /// <summary>
        /// Current order status
        /// </summary>
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// Target price
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// Executed average price
        /// </summary>
        public double? PriceAvg { get; set; }

        /// <summary>
        /// Target trailing price
        /// </summary>
        public double? PriceTrailing { get; set; }

        /// <summary>
        /// Auxiliary Limit price (for STOP LIMIT)
        /// </summary>
        public double? PriceAuxLimit { get; set; }

        /// <summary>
        /// Should notify (obsolete, see flags)
        /// </summary>
        public int? Notify { get; set; }

        /// <summary>
        /// Is hidden order (obsolete, see flags)
        /// </summary>
        public int? Hidden { get; set; }

        /// <summary>
        /// If another order caused this order to be placed (OCO) this will be that other order's ID
        /// </summary>
        public int? PlacedId { get; set; }

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


        internal static void Handle(JToken token, Subject<Order[]> subject)
        {
            var data = token[2];
            if (data.Type != JTokenType.Array)
            {
                Log.Warning(L("Orders - Invalid message format, third param not array"));
                return;
            }

            var parsed = data.ToObject<Order[]>();
            subject.OnNext(parsed);
        }

        internal static void Handle(JToken token, Subject<Order> subject)
        {
            var data = token[2];
            if (data.Type != JTokenType.Array)
            {
                Log.Warning(L("Order info - Invalid message format, third param not array"));
                return;
            }

            var parsed = data.ToObject<Order>();

            subject.OnNext(parsed);
        }

        private static string L(string msg)
        {
            return $"[BFX ORDER HANDLER] {msg}";
        }
    }
}
