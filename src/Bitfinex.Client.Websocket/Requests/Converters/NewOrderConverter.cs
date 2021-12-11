using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bitfinex.Client.Websocket.Exceptions;
using Bitfinex.Client.Websocket.Requests.Orders;
using Bitfinex.Client.Websocket.Responses.Orders;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests.Converters;
/*
 [
    0,
    "on",
    null,
    {
    "gid": 999,
    "cid": 999,
    "type": "EXCHANGE TRAILING STOP",
    "symbol": "tETHUSD",
    "amount": "0.2",
    "price": "10"
    }
]
 
 */

class NewOrderConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (!(value is NewOrderRequest request))
            throw new BitfinexBadInputException("Can't serialize new order request");

        writer.WriteStartArray();
        writer.WriteValue(0);
        writer.WriteValue("on");
        writer.WriteValue((object)null);

        writer.WriteStartObject();

        if (request.Gid.HasValue)
        {
            writer.WritePropertyName("gid");
            writer.WriteValue(request.Gid.Value);
        }

        writer.WritePropertyName("cid");
        writer.WriteValue(request.Cid);

        writer.WritePropertyName("type");
        writer.WriteValue(OrderConverter.SerializeType(request.Type));

        writer.WritePropertyName("symbol");
        writer.WriteValue(request.Symbol);

        writer.WritePropertyName("amount");
        writer.WriteValue(request.Amount.ToString(CultureInfo.InvariantCulture));

        if (request.Price.HasValue)
        {
            writer.WritePropertyName("price");
            writer.WriteValue(request.Price.Value.ToString(CultureInfo.InvariantCulture));
        }

        if (request.PriceTrailing.HasValue)
        {
            writer.WritePropertyName("price_trailing");
            writer.WriteValue(request.PriceTrailing.Value.ToString(CultureInfo.InvariantCulture));
        }

        if (request.PriceAuxLimit.HasValue)
        {
            writer.WritePropertyName("price_aux_limit");
            writer.WriteValue(request.PriceAuxLimit.Value.ToString(CultureInfo.InvariantCulture));
        }

        if (request.PriceOcoStop.HasValue)
        {
            writer.WritePropertyName("price_oco_stop");
            writer.WriteValue(request.PriceOcoStop.Value.ToString(CultureInfo.InvariantCulture));
        }

        if (request.Flags.HasValue)
        {
            writer.WritePropertyName("flags");
            writer.WriteValue((int)request.Flags);
        }

        if (request.TimeInForce.HasValue)
        {
            writer.WritePropertyName("tif");
            writer.WriteValue(request.TimeInForce.Value.ToString("u"));
        }

        if (request.Meta != null && request.Meta.Any())
        {
            writer.WritePropertyName("meta");
            WriteMeta(request.Meta, writer);
        }

        writer.WriteEndObject();
        writer.WriteEndArray();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(NewOrderRequest);
    }

    void WriteMeta(Dictionary<string, object> meta, JsonWriter writer)
    {
        writer.WriteStartObject();

        foreach (var param in meta)
        {
            writer.WritePropertyName(param.Key);
            writer.WriteValue(param.Value);
        }

        writer.WriteEndObject();
    }
}