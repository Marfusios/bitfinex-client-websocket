using System;
using Bitfinex.Client.Websocket.Exceptions;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests.Converters
{
    [JsonConverter(typeof(NewOrderConverter))]
    class CancelOrderConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is CancelOrderRequest order))
                throw new BitfinexBadInputException("Can't serialize order");


            writer.WriteStartArray();
            writer.WriteValue(0);
            writer.WriteValue("oc");
            writer.WriteValue((object)null);

            writer.WriteStartObject();

            writer.WritePropertyName("id");
            writer.WriteValue(order.Id);

            writer.WriteEndObject();
            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(CancelOrderRequest);
        }
    }
}
