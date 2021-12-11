using System;
using Bitfinex.Client.Websocket.Json;
using Bitfinex.Client.Websocket.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Positions;

class PositionConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Position);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        var array = JArray.Load(reader);
        return JArrayToPosition(array);
    }

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    Position JArrayToPosition(JArray array)
    {
        return new Position
        {
            Symbol = (string)array[0],
            Status = ParseStatus((string)array[1]),
            Amount = (double)array[2],
            BasePrice = (double)array[3],
            MarginFunding = (double)array[4],
            MarginFundingType = ParseFundingType((int?)array[5]),
            ProfitLoss = (double?)array[6],
            ProfitLossPercentage = (double?)array[7],
            LiquidationPrice = (double?)array[8]
        };
    }

    public static PositionStatus ParseStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return PositionStatus.Undefined;
        var safe = status.ToLower().Trim();
        switch (safe)
        {
            case "active":
            case var s when s.Contains("active"):
                return PositionStatus.Active;
            case "closed":
            case var s when s.Contains("closed"):
                return PositionStatus.Closed;
        }
        BitfinexJsonSerializer.AuthenticatedLogger.LogWarning(BitfinexLogMessage.Authenticated("Can't parse PositionStatus, input: " + safe));
        return PositionStatus.Undefined;
    }

    public static MarginFundingType ParseFundingType(int? type)
    {
        if (!type.HasValue)
            return MarginFundingType.Undefined;
        switch (type.Value)
        {
            case 0:
                return MarginFundingType.Daily;
            case 1:
                return MarginFundingType.Term;
        }
        BitfinexJsonSerializer.AuthenticatedLogger.LogWarning(BitfinexLogMessage.Authenticated("Can't parse MarginFundingType, input: " + type));
        return MarginFundingType.Undefined;
    }
}