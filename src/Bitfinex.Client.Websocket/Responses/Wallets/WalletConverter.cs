using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Wallets
{
    class WalletConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Wallet);
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

        private Wallet JArrayToTradingTicker(JArray array)
        {
            return new Wallet
            {
                Type = ParseWalletType((string)array[0]),
                Currency = (string)array[1],
                Balance = (double)array[2],
                UnsettledInterest = (double)array[3],
                BalanceAvailable = (double?)array[4]
            };
        }

        public static WalletType ParseWalletType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return WalletType.Undefined;
            var safe = type.Trim();
            if (safe.StartsWith("exchange", StringComparison.OrdinalIgnoreCase))
                return WalletType.Exchange;
            if (safe.StartsWith("margin", StringComparison.OrdinalIgnoreCase))
                return WalletType.Margin;
            if (safe.StartsWith("funding", StringComparison.OrdinalIgnoreCase))
                return WalletType.Funding;

            return WalletType.Undefined;
        }
    }
}
