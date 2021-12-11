using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Bitfinex.Client.Websocket.Responses.FundingTradesPrivate;

public class PrivateFundingTradeConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(PrivateFundingTrade);
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

    PrivateFundingTrade JArrayToTradingTicker(JArray array)
    {
        return new PrivateFundingTrade
        {
            Id = (long)array[0],
            Symbol = (string)array[1],
            MtsCreate = BitfinexTime.ConvertToTime((long)array[2]),
            OfferId = (long)array[3],
            Amount = (double)array[4],
            Rate = (double)array[5],
            Period = (int)array[6],
            IsMaker = (int)array[7] > 0
        };
    }
}