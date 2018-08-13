using Bitfinex.Client.Websocket.Json;
using Bitfinex.Client.Websocket.Responses.Trades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bitfinex.Client.Websocket.Responses.Fundings
{

    public enum FundingType
    {
        Executed,
        UpdateExecution
    }

    /// <summary>
    /// The order that causes the trade determines if it is a buy or a sell.
    /// </summary>
    [JsonConverter(typeof(FundingConverter))]
    public class Funding : ResponseBase
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
        /// Rate at which funding transaction occurred
        /// </summary>
        public double Rate { get; set; }

        /// <summary>
        /// Amount of time the funding transaction was for
        /// </summary>
        public double Period { get; set; }

        [JsonIgnore]
        public FundingType Type { get; set; }

        /// <summary>
        /// Target Symbol
        /// </summary>        
        [JsonIgnore]
        public string Symbol { get; set; }

    }
    
}
