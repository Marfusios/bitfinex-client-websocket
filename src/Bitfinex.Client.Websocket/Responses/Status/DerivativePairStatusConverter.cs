using System;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Status
{
    class DerivativePairStatusConverter : JsonConverter
    {
        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            return JArrayToDerivativePairStatus(JArray.Load(reader));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DerivativePairStatus);
        }

        private DerivativePairStatus JArrayToDerivativePairStatus(JToken jToken)
        {
            return new DerivativePairStatus
            {
                TimestampMs = BitfinexTime.ConvertToTime((long) jToken[0]),
                DerivPrice = (double) jToken[2],
                SpotPrice = (double) jToken[3],
                InsuranceFundBalance = (double) jToken[5],
                FundingAccrued = (double) jToken[8],
                FundingStep = (long) jToken[9],
            };
        }
    }
}