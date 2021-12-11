using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Books;

class RawBookConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(RawBook);
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

    RawBook JArrayToTradingTicker(JArray array)
    {
        if (array.Count == 3)
        {
            return new RawBook
            {
                OrderId = (long)array[0],
                Price = (double)array[1],
                Amount = (double)array[2],
            };
        }

        if (array.Count > 3)
        {
            return new RawBook
            {
                OfferId = (long)array[0],
                Period = (int)array[1],
                Rate = (double)array[2],
                Amount = (double)array[3],
            };
        }

        return null;
    }
}