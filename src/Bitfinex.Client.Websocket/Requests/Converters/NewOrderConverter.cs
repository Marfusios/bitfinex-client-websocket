using System;
using System.Globalization;
using Bitfinex.Client.Websocket.Exceptions;
using Bitfinex.Client.Websocket.Responses.Orders;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests.Converters
{
    class NewOrderConverter : JsonConverter
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

            writer.WritePropertyName("gid");
            writer.WriteValue(order.Gid);

            writer.WritePropertyName("cid");
            writer.WriteValue(order.Cid);

            writer.WritePropertyName("type");
            writer.WriteValue(OrderConverter.SerializeType(order.Type));

            writer.WritePropertyName("symbol");
            writer.WriteValue(order.Symbol);

            writer.WritePropertyName("amount");
            writer.WriteValue(order.Amount.ToString(CultureInfo.InvariantCulture));

            writer.WritePropertyName("price");
            writer.WriteValue(order.Price.ToString(CultureInfo.InvariantCulture));

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
