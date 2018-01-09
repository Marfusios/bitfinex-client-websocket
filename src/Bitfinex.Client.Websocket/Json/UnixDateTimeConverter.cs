using System;
using System.Globalization;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bitfinex.Client.Websocket.Json
{
    public class UnixDateTimeConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var substracted = ((DateTime)value).Subtract(BitfinexTime.UnixBase);
            writer.WriteRawValue(substracted.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            return BitfinexTime.UnixBase.AddMilliseconds((long)reader.Value);
        }
    }
}
