using System;
using Bitfinex.Client.Websocket.Exceptions;
using Bitfinex.Client.Websocket.Requests.Orders;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests.Converters
{
    internal class CancelMultiOrderConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is CancelMultiOrderRequest request))
                throw new BitfinexBadInputException("Can't serialize cancel multi order request");


            writer.WriteStartArray();
            writer.WriteValue(0);
            writer.WriteValue("oc_multi");
            writer.WriteValue((object)null);

            writer.WriteStartObject();

            if (request.CancelAll)
            {
                writer.WritePropertyName("all");
                writer.WriteValue(1);
            }

            if (request.Ids != null)
            {
                writer.WritePropertyName("id");
                writer.WriteStartArray();

                foreach (var id in request.Ids)
                {
                    writer.WriteValue(id);
                }

                writer.WriteEndArray();
            }

            if (request.Gids != null)
            {
                writer.WritePropertyName("gid");
                writer.WriteStartArray();

                foreach (var id in request.Gids)
                {
                    writer.WriteValue(id);
                }

                writer.WriteEndArray();
            }

            if (request.CidPairs != null)
            {
                writer.WritePropertyName("cid");
                writer.WriteStartArray();

                foreach (var cidPair in request.CidPairs)
                {
                    if(cidPair == null)
                        continue;

                    writer.WriteStartArray();
                    writer.WriteValue(cidPair.Cid);
                    writer.WriteValue(cidPair.CidDate.ToString("yyyy-MM-dd"));
                    writer.WriteEndArray();
                }

                writer.WriteEndArray();
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
            return objectType == typeof(CancelMultiOrderRequest);
        }
    }
}
