using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Books;

class BookConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Book);
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

    Book JArrayToTradingTicker(JArray array)
    {
        if (array.Count == 3)
        {
            return new Book
            {
                Price = (double)array[0],
                Count = (int)array[1],
                Amount = (double)array[2],
            };
        }

        if (array.Count > 3)
        {
            return new Book
            {
                Rate = (double)array[0],
                Period = (double)array[1],
                Count = (int)array[2],
                Amount = (double)array[3],
            };
        }

        return null;
    }
}