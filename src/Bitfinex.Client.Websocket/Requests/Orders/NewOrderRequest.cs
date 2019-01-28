using System;
using Bitfinex.Client.Websocket.Requests.Converters;
using Bitfinex.Client.Websocket.Requests.Orders;
using Bitfinex.Client.Websocket.Responses.Orders;
using Bitfinex.Client.Websocket.Validations;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests
{
    /// <summary>
    /// Request to create a new order.
    /// You will receive a message of the appropriated type on the order streams
    /// </summary>
    [JsonConverter(typeof(NewOrderConverter))]
    public class NewOrderRequest
    {
        private string _symbol;

        /// <summary>
        /// Don't forget to set relevant properties
        /// </summary>
        public NewOrderRequest()
        {
            
        }

        /// <summary>
        /// Simple constructor mostly for LIMIT and MARKET order,
        /// for other order types use parameter-less constructor and set properties by your own 
        /// </summary>
        public NewOrderRequest(long gid, long cid, string symbol, OrderType type, double amount, double price)
        {
            BfxValidations.ValidateInput(cid, nameof(cid), 0);
            BfxValidations.ValidateInput((int)type, nameof(type), 0);
            BfxValidations.ValidateInput(price, nameof(price), 0);
            BfxValidations.ValidateInput(symbol, nameof(symbol));

            Gid = gid;
            Cid = cid;
            Symbol = symbol;
            Type = type;
            Amount = amount;
            Price = price;
        }

        /// <summary>
        /// (optional) Group id for the order
        /// </summary>
        public long? Gid { get; set; }

        /// <summary>
        /// Must be unique in the day (UTC)
        /// </summary>
        public long Cid { get; set; }

        /// <summary>
        /// symbol (tBTCUSD, tETHUSD, ...)
        /// </summary>
        public string Symbol
        {
            get => _symbol;
            set
            {
                var s = value ?? string.Empty;
                _symbol = (s.StartsWith("t") ? s : "t" + s).Replace("/", string.Empty);
            }
        }

        /// <summary>
        /// Type of the order
        /// </summary>
        public OrderType Type { get; set; }

        /// <summary>
        /// Positive for buy, Negative for sell
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Price (Not required for market orders)
        /// </summary>
        public double? Price { get; set; }

        /// <summary>
        /// The trailing price
        /// </summary>
        public double? PriceTrailing { get; set; }

        /// <summary>
        /// Auxiliary Limit price (for STOP LIMIT)
        /// </summary>
        public double? PriceAuxLimit { get; set; }

        /// <summary>
        /// OCO stop price
        /// </summary>
        public double? PriceOcoStop { get; set; }

        /// <summary>
        /// Additional order configuration, see OrderFlag enum. 
        /// You may sum flag values to pass multiple flags. For example passing 4160 (64 + 4096) means hidden post only.
        /// Use C# [Flags] to do that: Flags = OrderFlag.Hidden | OrderFlag.PostOnly
        /// </summary>
        public OrderFlag? Flags { get; set; }

        /// <summary>
        /// Time-In-Force: datetime for automatic order cancellation (ie. 2020-01-01 10:45:23) )
        /// </summary>
        public DateTime? TimeInForce { get; set; }
    }
}
