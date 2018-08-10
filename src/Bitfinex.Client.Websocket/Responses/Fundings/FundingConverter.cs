using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bitfinex.Client.Websocket.Responses.Fundings
{
    public class FundingConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Funding);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var array = JArray.Load(reader);
            return JArrayToTradingTicker(array);
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private Funding JArrayToTradingTicker(JArray array)
        {
            return new Funding
            {
                Id = (long)array[0],
                Mts = BitfinexTime.ConvertToTime((long)array[1]),
                Amount = (double)array[2],
                Rate = (double)array[3],
                Period = (int)array[4]
            };
        }
    }
}
