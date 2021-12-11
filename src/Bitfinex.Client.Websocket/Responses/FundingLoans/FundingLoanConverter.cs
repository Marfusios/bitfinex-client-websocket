using System;
using Bitfinex.Client.Websocket.Responses.FundingOffers;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.FundingLoans;

class FundingLoanConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(FundingLoan);
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

    FundingLoan JArrayToTradingTicker(JArray array)
    {
        return new FundingLoan
        {
            Id = (long)array[0],
            Symbol = (string)array[1],
            Side = (FundingSide)(int)array[2],
            MtsCreate = BitfinexTime.ConvertToTime((long?)array[3]),
            MtsUpdate = BitfinexTime.ConvertToTime((long?)array[4]),
            Amount = (double?)array[5],
            Flags = (int?)array[6],
            Status = FundingOfferConverter.ParseStatus((string)array[7]),
            Rate = (double)array[11],
            Period = (double)array[12],
            MtsOpening = BitfinexTime.ConvertToTime((long?)array[13]),
            MtsLastPayout = BitfinexTime.ConvertToTime((long?)array[14]),
            Notify = (int)array[15] > 0,
            Hidden = (int)array[16] > 0,
            Renew = (int)array[18] > 0,
            RateReal = (double)array[19],
            NoClose = (int)array[20] > 0
        };
    }
}