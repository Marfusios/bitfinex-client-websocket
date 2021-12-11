using System;
using Bitfinex.Client.Websocket.Exceptions;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests.Converters;

public class CalcConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (!(value is CalcRequest calcRequest))
            throw new BitfinexBadInputException("Can't serialize calcRequest");

        writer.WriteStartArray();
        writer.WriteValue(0);
        writer.WriteValue("calc");
        writer.WriteValue((object) null);

        writer.WriteStartArray();

        foreach (var request in calcRequest.Requests)
        {
            writer.WriteStartArray();
            writer.WriteValue(request);
            writer.WriteEndArray();
        }

        writer.WriteEndArray();
        writer.WriteEndArray();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(CalcRequest);
    }
}