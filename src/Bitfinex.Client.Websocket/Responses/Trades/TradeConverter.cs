using System;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Trades
{
    internal class TradeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Trade);
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

        private Trade JArrayToTradingTicker(JArray array)
        {
            return new Trade
            {
                Id = (long)array[0],
                Mts = BitfinexTime.ConvertToTime((long)array[1]),
                Amount = (double)array[2],
                Price = (double)array[3]
            };
        }
    }
}
