using System;
using System.Globalization;
using Bitfinex.Client.Websocket.Exceptions;
using Bitfinex.Client.Websocket.Requests.Orders;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests.Converters
{
    internal class UpdateOrderConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is UpdateOrderRequest request))
                throw new BitfinexBadInputException("Can't serialize update order request");

            
            writer.WriteStartArray();
            writer.WriteValue(0);
            writer.WriteValue("ou");
            writer.WriteValue((object)null);

            writer.WriteStartObject();

            if (request.Id.HasValue)
            {
                writer.WritePropertyName("id");
                writer.WriteValue(request.Id);
            }

            if (request.CidPair != null)
            {
                writer.WritePropertyName("cid");
                writer.WriteValue(request.CidPair.Cid);

                writer.WritePropertyName("cid_date");
                writer.WriteValue(request.CidPair.CidDate.ToString("yyyy-MM-dd"));
            }

            if (request.Gid.HasValue)
            {
                writer.WritePropertyName("gid");
                writer.WriteValue(request.Gid.Value);
            }

            if (request.Price.HasValue)
            {
                writer.WritePropertyName("price");
                writer.WriteValue(request.Price.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (request.Amount.HasValue)
            {
                writer.WritePropertyName("amount");
                writer.WriteValue(request.Amount.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (request.Delta.HasValue)
            {
                writer.WritePropertyName("delta");
                writer.WriteValue(request.Delta.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (request.PriceTrailing.HasValue)
            {
                writer.WritePropertyName("price_trailing");
                writer.WriteValue(request.PriceTrailing.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (request.PriceAuxLimit.HasValue)
            {
                writer.WritePropertyName("price_aux_limit");
                writer.WriteValue(request.PriceAuxLimit.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (request.Flags.HasValue)
            {
                writer.WritePropertyName("flags");
                writer.WriteValue((int)request.Flags);
            }

            if (request.TimeInForce.HasValue)
            {
                writer.WritePropertyName("tif");
                writer.WriteValue(request.TimeInForce.Value.ToString("u"));
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
            return objectType == typeof(UpdateOrderRequest);
        }
    }
}
