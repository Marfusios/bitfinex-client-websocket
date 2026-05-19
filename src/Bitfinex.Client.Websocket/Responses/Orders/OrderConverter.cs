using System;
using Bitfinex.Client.Websocket.Exceptions;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Orders
{
    internal class OrderConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Order);
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

        private Order JArrayToTradingTicker(JArray array)
        {
            return new Order
            {
                Id = (long)array[0],
                Gid = (long?)array[1],
                Cid = (long)array[2],
                Symbol = (string)array[3],
                MtsCreate = BitfinexTime.ConvertToTime((long?)array[4]),
                MtsUpdate = BitfinexTime.ConvertToTime((long?)array[5]),
                Amount = (double?)array[6],
                AmountOrig = (double?)array[7],
                Type = ParseType((string)array[8]),
                TypePrev = ParseType((string)array[9]),
                MtsTiff = BitfinexTime.ConvertToTime((long?)array[10]),
                // 11
                Flags = (int?)array[12],
                OrderStatus = ParseStatus((string)array[13]),
                OrderStatusText = (string)array[13],
                // 14
                // 15
                Price = (double?)array[16],
                PriceAvg = (double?)array[17],
                PriceTrailing = (double?)array[18],
                PriceAuxLimit = (double?)array[19],
                // 20
                // 21
                // 22
                Notify = (int?)array[23],
                Hidden = (int?)array[24],
                PlacedId = (long?)array[25],
            };
        }

        public static OrderStatus ParseStatus(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return OrderStatus.Undefined;
            var safe = status.Trim();
            if (safe.StartsWith("active", StringComparison.OrdinalIgnoreCase))
                return OrderStatus.Active;
            if (safe.StartsWith("executed", StringComparison.OrdinalIgnoreCase))
                return OrderStatus.Executed;
            if (safe.IndexOf("postonly canceled", StringComparison.OrdinalIgnoreCase) >= 0)
                return OrderStatus.PostOnlyCanceled;
            if (safe.IndexOf("rsn_pos_reduce_flip", StringComparison.OrdinalIgnoreCase) >= 0)
                return OrderStatus.RsnPosReduceFlip;
            if (safe.IndexOf("rsn_pos_reduce_incr", StringComparison.OrdinalIgnoreCase) >= 0)
                return OrderStatus.RsnPosReduceIncr;
            if (safe.IndexOf("insufficient balance", StringComparison.OrdinalIgnoreCase) >= 0)
                return OrderStatus.InsufficientBalance;
            if (safe.IndexOf("insufficient margin", StringComparison.OrdinalIgnoreCase) >= 0)
                return OrderStatus.InsufficientMargin;
            if (safe.IndexOf("canceled", StringComparison.OrdinalIgnoreCase) >= 0)
                return OrderStatus.Canceled;

            // must be last, because of statuses like: 'CANCELED was: PARTIALLY FILLED @ PRICE(AMOUNT)',
            // or 'INSUFFICIENT MARGIN was: PARTIALLY FILLED @ PRICE(AMOUNT)'
            if (safe.IndexOf("partially filled", StringComparison.OrdinalIgnoreCase) >= 0)
                return OrderStatus.PartiallyFilled;

            return OrderStatus.Undefined;
        }

        public static OrderType ParseType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return OrderType.Undefined;
            var safe = type.Trim();
            if (safe.StartsWith("market", StringComparison.OrdinalIgnoreCase))
                return OrderType.Market;
            if (safe.StartsWith("exchange market", StringComparison.OrdinalIgnoreCase))
                return OrderType.ExchangeMarket;
            if (safe.StartsWith("limit", StringComparison.OrdinalIgnoreCase))
                return OrderType.Limit;
            if (safe.StartsWith("exchange limit", StringComparison.OrdinalIgnoreCase))
                return OrderType.ExchangeLimit;
            if (safe.StartsWith("trailing stop", StringComparison.OrdinalIgnoreCase))
                return OrderType.TrailingStop;
            if (safe.StartsWith("exchange trailing stop", StringComparison.OrdinalIgnoreCase))
                return OrderType.ExchangeTrailingStop;
            if (safe.StartsWith("stop", StringComparison.OrdinalIgnoreCase))
                return OrderType.Stop;
            if (safe.StartsWith("exchange stop", StringComparison.OrdinalIgnoreCase))
                return OrderType.ExchangeStop;
            if (safe.StartsWith("stop limit", StringComparison.OrdinalIgnoreCase))
                return OrderType.StopLimit;
            if (safe.StartsWith("exchange stop limit", StringComparison.OrdinalIgnoreCase))
                return OrderType.ExchangeStopLimit;
            if (safe.StartsWith("fok", StringComparison.OrdinalIgnoreCase))
                return OrderType.Fok;
            if (safe.StartsWith("exchange fok", StringComparison.OrdinalIgnoreCase))
                return OrderType.ExchangeFok;

            return OrderType.Undefined;
        }

        public static string SerializeType(OrderType type)
        {
            switch (type)
            {
                case OrderType.Market:
                    return "MARKET";
                case OrderType.ExchangeMarket:
                    return "EXCHANGE MARKET";
                case OrderType.Limit:
                    return "LIMIT";
                case OrderType.ExchangeLimit:
                    return "EXCHANGE LIMIT";
                case OrderType.TrailingStop:
                    return "TRAILING STOP";
                case OrderType.ExchangeTrailingStop:
                    return "EXCHANGE TRAILING STOP";
                case OrderType.Stop:
                    return "STOP";
                case OrderType.ExchangeStop:
                    return "EXCHANGE STOP";
                case OrderType.StopLimit:
                    return "STOP LIMIT";
                case OrderType.ExchangeStopLimit:
                    return "EXCHANGE STOP LIMIT";
                case OrderType.Fok:
                    return "FOK";
                case OrderType.ExchangeFok:
                    return "EXCHANGE FOK";
            }
            throw new BitfinexException("Not supported order type");
        }
    }
}
