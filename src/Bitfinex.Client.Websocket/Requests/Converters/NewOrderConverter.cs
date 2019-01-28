using System;
using System.Globalization;
using Bitfinex.Client.Websocket.Exceptions;
using Bitfinex.Client.Websocket.Responses.Orders;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests.Converters
{
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

    internal class NewOrderConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is NewOrderRequest order))
                throw new BitfinexBadInputException("Can't serialize order");

            
            writer.WriteStartArray();
            writer.WriteValue(0);
            writer.WriteValue("on");
            writer.WriteValue((object)null);

            writer.WriteStartObject();

            if (order.Gid.HasValue)
            {
                writer.WritePropertyName("gid");
                writer.WriteValue(order.Gid.Value);
            }

            writer.WritePropertyName("cid");
            writer.WriteValue(order.Cid);

            writer.WritePropertyName("type");
            writer.WriteValue(OrderConverter.SerializeType(order.Type));

            writer.WritePropertyName("symbol");
            writer.WriteValue(order.Symbol);

            writer.WritePropertyName("amount");
            writer.WriteValue(order.Amount.ToString(CultureInfo.InvariantCulture));

            if (order.Price.HasValue)
            {
                writer.WritePropertyName("price");
                writer.WriteValue(order.Price.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (order.PriceTrailing.HasValue)
            {
                writer.WritePropertyName("price_trailing");
                writer.WriteValue(order.PriceTrailing.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (order.PriceAuxLimit.HasValue)
            {
                writer.WritePropertyName("price_aux_limit");
                writer.WriteValue(order.PriceAuxLimit.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (order.PriceOcoStop.HasValue)
            {
                writer.WritePropertyName("price_oco_stop");
                writer.WriteValue(order.PriceOcoStop.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (order.Flags.HasValue)
            {
                writer.WritePropertyName("flags");
                writer.WriteValue((int)order.Flags);
            }

            if (order.TimeInForce.HasValue)
            {
                writer.WritePropertyName("tif");
                writer.WriteValue(order.TimeInForce.Value.ToString("u"));
            }

            writer.WriteEndObject();
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(NewOrderRequest);
        }
    }
}
