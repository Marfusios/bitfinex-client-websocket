using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Responses.Books
{
    /// <summary>
    /// The state of the Bitfinex order book
    /// </summary>
    [JsonConverter(typeof(BookConverter))]
    public class Book : ResponseBase
    {
        /// <summary>
        /// Price level
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Number of orders at that price level
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Total amount available at that price level. 
        /// Trading: if AMOUNT greater than 0 then bid else ask; 
        /// Funding: if AMOUNT lower than 0 then bid else ask;
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Rate level
        /// </summary>
        public double Rate { get; set; }

        /// <summary>
        /// Period level
        /// </summary>
        public double Period { get; set; }

        /// <summary>
        /// Target pair
        /// </summary>
        [JsonIgnore]
        public string Pair { get; set; }
    }
}
