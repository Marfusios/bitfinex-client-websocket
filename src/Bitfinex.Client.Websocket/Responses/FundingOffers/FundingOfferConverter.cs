using System;
using Bitfinex.Client.Websocket.Json;
using Bitfinex.Client.Websocket.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.FundingOffers;

class FundingOfferConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(FundingOffer);
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

    FundingOffer JArrayToTradingTicker(JArray array)
    {
        return new FundingOffer
        {
            Id = (long)array[0],
            Symbol = (string)array[1],
            MtsCreate = BitfinexTime.ConvertToTime((long?)array[2]),
            MtsUpdate = BitfinexTime.ConvertToTime((long?)array[3]),
            Amount = (double?)array[4],
            AmountOrig = (double?)array[5],
            OfferType = ParseType((string)array[6]),
            Flags = (int?)array[9],
            Status = ParseStatus((string)array[10]),
            Rate = (double)array[14],
            Period = (double)array[15],
            Notify = (int)array[16] > 0,
            Hidden = (int)array[17] > 0,
            Renew = (int)array[19] > 0
        };
    }

    public static FundingStatus ParseStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return FundingStatus.Undefined;
        var safe = status.ToLower().Trim();
        switch (safe)
        {
            case "active":
                return FundingStatus.Active;
            case "executed":
                return FundingStatus.Executed;
            case "canceled":
                return FundingStatus.Canceled;
            case "partially filled":
                return FundingStatus.PartiallyFilled;

        }
        BitfinexJsonSerializer.AuthenticatedLogger.LogWarning(BitfinexLogMessage.Authenticated("Can't parse FundingStatus, input: " + safe));
        return FundingStatus.Undefined;
    }

    public static FundingOfferType ParseType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            return FundingOfferType.Undefined;
        var safe = type.ToLower().Trim();
        switch (safe)
        {
            case "limit":
                return FundingOfferType.Limit;
            case "frrdeltavar":
                return FundingOfferType.FrrDeltaVar;
        }
        BitfinexJsonSerializer.AuthenticatedLogger.LogWarning(BitfinexLogMessage.Authenticated("Can't parse FundingOfferType, input: " + safe));
        return FundingOfferType.Undefined;
    }
}