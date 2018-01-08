using Bitfinex.Client.Websocket.Requests.Converters;
using Bitfinex.Client.Websocket.Responses.Orders;
using Bitfinex.Client.Websocket.Validations;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests
{
    [JsonConverter(typeof(NewOrderConverter))]
    public class NewOrderRequest
    {
        public NewOrderRequest(long gid, long cid, string symbol, OrderType type, double amount, double price)
        {
            BfxValidations.ValidateInput(cid, nameof(cid), 0);
            BfxValidations.ValidateInput((int)type, nameof(type), 0);
            BfxValidations.ValidateInput(price, nameof(price), 0);
            BfxValidations.ValidateInput(symbol, nameof(symbol));

            Gid = gid;
            Cid = cid;
            Symbol = (symbol.StartsWith("t") ? symbol : "t" + symbol).Replace("/", string.Empty);
            Type = type;
            Amount = amount;
            Price = price;
        }

        public long Gid { get; }
        public long Cid { get; }
        public string Symbol { get; }
        public OrderType Type { get;}
        public double Amount { get; }
        public double Price { get; }
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
