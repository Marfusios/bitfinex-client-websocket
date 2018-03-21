using Bitfinex.Client.Websocket.Requests.Converters;
using Bitfinex.Client.Websocket.Responses.Orders;
using Bitfinex.Client.Websocket.Validations;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests
{
    [JsonConverter(typeof(NewOrderConverter))]
    public class NewOrderRequest
    {
        private string _symbol;

        public NewOrderRequest()
        {
            
        }

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
        /// Whether the order is hidden (1) or not (0)
        /// </summary>
        public int Hidden { get; set; }

        /// <summary>
        /// You may sum flag values to pass multiple flags. For example passing 4160 (64 + 4096) means hidden post only.
        /// ------------------------------------------------------------------------------------------------------------
        /// 64	    The hidden order option ensures an order does not appear in the order book; thus does not influence other market participants.
        /// 512	    Close position if position present.
        /// 4096	The post-only limit order option ensures the limit order will be added to the order book and not match with a pre-existing order.
        /// 16384	The one cancels other order option allows you to place a pair of orders stipulating that if one order is executed fully or partially, then the other is automatically canceled.
        /// </summary>
        public int? Flags { get; set; }
    }

    /*
     [
        0,
        "on",
        null,
        {
        "gid": 999,
        "cid": 999,
        "type": "EXCHANGE TRAILING STOP",
        "symbol": "tETHUSD",
        "amount": "0.2",
        "price": "10"
        }
    ]
     
     */
}
