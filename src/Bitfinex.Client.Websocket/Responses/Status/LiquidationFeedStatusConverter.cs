using System;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Status;

class LiquidationFeedStatusConverter : JsonConverter
{
    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        return JArrayToDerivativePairStatus(JArray.Load(reader));
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(LiquidationFeedStatus);
    }

    LiquidationFeedStatus JArrayToDerivativePairStatus(JToken jToken)
    {
        return new LiquidationFeedStatus
        {
            Type = (string) jToken[0],
            PosId = (long) jToken[1],
            TimestampMs = BitfinexTime.ConvertToTime((long) jToken[2]),
            Symbol = (string) jToken[4],
            Amount = (double) jToken[5],
            BasePrice = (double) jToken[6],
            IsMatch = (int) jToken[8],
            IsMarketSold = (int) jToken[9],
        };
    }
}