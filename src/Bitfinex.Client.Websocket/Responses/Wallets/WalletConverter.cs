using System;
using Bitfinex.Client.Websocket.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Wallets
{
    class WalletConverter : JsonConverter
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger(); 

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
            var safe = type.ToLower().Trim();
            switch (safe)
            {
                case "exchange":
                case var s when s.StartsWith("exchange"):
                    return WalletType.Exchange;
                case "margin":
                case var s when s.StartsWith("margin"):
                    return WalletType.Margin;
                case "funding":
                case var s when s.StartsWith("funding"):
                    return WalletType.Funding;
            }
            Log.Warn("Can't parse WalletType, input: " + safe);
            return WalletType.Undefined;
        }
    }
}
