using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Balance;

class BalanceInfoConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(BalanceInfo);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        var array = JArray.Load(reader);
        return JArrayToBalanceInfo(array);
    }

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    BalanceInfo JArrayToBalanceInfo(JArray array)
    {
        return new BalanceInfo
        {
            TotalAum = (double) array[0],
            NetAum = (double) array[1]
        };
    }
}