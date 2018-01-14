using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Candles
{
    public class CandlesConverter : JsonConverter
    {
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

            if (jArray.Count==6)
            {
                candles.CandleList.Add(JArrayToCandle(jArray));
            }
            else
            {
                foreach (var candle in jArray)
                {
                    candles.CandleList.Add(JArrayToCandle(candle));
                }
            }

            return candles;
        }

        private Candle JArrayToCandle(JToken jToken)
        {
            return new Candle
            {
                Mts = Int64.Parse(jToken[0].ToString()),
                Open = (double) jToken[1],
                Close = (double) jToken[2],
                High = (double) jToken[3],
                Low = (double) jToken[4],
                Volume = (double) jToken[5]
            };
        }
    }
}
