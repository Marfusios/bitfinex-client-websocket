using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Bitfinex.Client.Websocket.Responses.FundingTrades;

public class FundingTradeConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(FundingTrade);
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

    FundingTrade JArrayToTradingTicker(JArray array)
    {
        return new FundingTrade
        {
            Id = (long)array[0],
            Mts = BitfinexTime.ConvertToTime((long)array[1]),
            Amount = (double)array[2],
            Rate = (double)array[3],
            Period = (int)array[4]
        };
    }
}