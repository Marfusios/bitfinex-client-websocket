using System;
using Bitfinex.Client.Websocket.Json;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Responses.Trades
{
    public enum TradeType
    {
        Executed,
        UpdateExecution
    }

    /// <summary>
    /// The order that causes the trade determines if it is a buy or a sell.
    /// </summary>
    [JsonConverter(typeof(TradeConverter))]
    public class Trade : ResponseBase
    {
        public long Id { get; set; }

        /// <summary>
        /// Millisecond time stamp
        /// </summary>
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Mts { get; set; }

        /// <summary>
        /// How much was bought (positive) or sold (negative).
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Price at which the trade was executed
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Rate at which funding transaction occurred
        /// </summary>
        public double Rate { get; set; }

        /// <summary>
        /// Amount of time the funding transaction was for
        /// </summary>
        public double Period { get; set; }

        [JsonIgnore]
        public TradeType Type { get; set; }

        /// <summary>
        /// Target pair
        /// </summary>
        [JsonIgnore]
        public string Pair { get; set; }
    }
}
