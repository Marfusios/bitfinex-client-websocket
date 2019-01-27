using System;
using Bitfinex.Client.Websocket.Responses.Orders;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.TradesPrivate
{
    internal class PrivateTradeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PrivateTrade);
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

        private PrivateTrade JArrayToTradingTicker(JArray array)
        {
            return new PrivateTrade
            {
                Id = (long)array[0],
                Symbol = (string)array[1],
                MtsCreate = BitfinexTime.ConvertToTime((long)array[2]),
                OrderId = (long)array[3],
                ExecAmount = (double)array[4],
                ExecPrice = (double)array[5],
                OrderType = OrderConverter.ParseType((string)array[6]),
                OrderPrice = (double)array[7],
                IsMaker = (int)array[8] > 0,
                Fee = (double?)array[9],
                FeeCurrency = (string)array[10]
            };
        }
    }
}
