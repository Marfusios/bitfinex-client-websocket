using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Margin
{
    internal class MarginInfoConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MarginInfo);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var array = JArray.Load(reader);

            if ((string) array[0] == "base")
            {
                return JArrayToMarginInfo(array[1] as JArray);
            }

            return null;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private MarginInfo JArrayToMarginInfo(JArray array)
        {
            return new MarginInfo()
            {
                UserPl = (double) array[0],
                UserSwaps = (double) array[1],
                MarginBalance = (double) array[2],
                MarginNet = (double) array[3],
                MarginRequired = (double) array[4],
            };
        }
    }
}