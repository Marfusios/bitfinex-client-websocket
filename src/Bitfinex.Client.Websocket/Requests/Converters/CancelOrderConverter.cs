using System;
using Bitfinex.Client.Websocket.Exceptions;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests.Converters
{
    internal class CancelOrderConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is CancelOrderRequest request))
                throw new BitfinexBadInputException("Can't serialize cancel order request");


            writer.WriteStartArray();
            writer.WriteValue(0);
            writer.WriteValue("oc");
            writer.WriteValue((object)null);

            writer.WriteStartObject();

            if (request.Id.HasValue)
            {
                writer.WritePropertyName("id");
                writer.WriteValue(request.Id.Value);
            }

            if (request.CidPair != null)
            {
                writer.WritePropertyName("cid");
                writer.WriteValue(request.CidPair.Cid);

                writer.WritePropertyName("cid_date");
                writer.WriteValue(request.CidPair.CidDate.ToString("yyyy-MM-dd"));
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
            return objectType == typeof(CancelOrderRequest);
        }
    }
}
