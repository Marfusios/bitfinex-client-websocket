using System;
using System.Collections.Generic;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Candles
{
    class CandlesConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return JArrayToCandles(JArray.Load(reader));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Candle);
        }

        private Candles JArrayToCandles(JArray jArray)
        {
            var candles = new Candles();
            var candleList = new List<Candle>();

            if (jArray.Count==6)
            {
                candleList.Add(JArrayToCandle(jArray));
            }
            else
            {
                foreach (var candle in jArray)
                {
                    candleList.Add(JArrayToCandle(candle));
                }
            }

            candles.CandleList = candleList.ToArray();
            return candles;
        }

        private Candle JArrayToCandle(JToken jToken)
        {
            return new Candle
            {
                Mts = BitfinexTime.ConvertToTime((long)jToken[0]),
                Open = (double) jToken[1],
                Close = (double) jToken[2],
                High = (double) jToken[3],
                Low = (double) jToken[4],
                Volume = (double) jToken[5]
            };
        }
    }
}
