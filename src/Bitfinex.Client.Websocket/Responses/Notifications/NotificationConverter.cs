using System;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Notifications
{
    class NotificationConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Notification);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var array = JArray.Load(reader);
            return JArrayToNotification(array);
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private Notification JArrayToNotification(JArray array)
        {
            return new Notification
            {
                Mts = BitfinexTime.ConvertToTime((long)array[0]),
                Type = ParseNotificationType((string)array[1]),
                MessageId = (long?)array[2],
                NotifyInfo = array[4].ToString(),
                Code = (long?)array[5],
                Status = (string)array[6],
                Text = array[7].ToString(),
            };
        }

        private static NotificationType ParseNotificationType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return NotificationType.Undefined;
            var safe = type.Trim();
            if (safe.Equals("on-req", StringComparison.OrdinalIgnoreCase))
                return NotificationType.OnReq;
            if (safe.Equals("oc-req", StringComparison.OrdinalIgnoreCase))
                return NotificationType.OcReq;
            if (safe.Equals("on_multi-req", StringComparison.OrdinalIgnoreCase))
                return NotificationType.OnMultiReq;
            if (safe.Equals("oc_multi-req", StringComparison.OrdinalIgnoreCase))
                return NotificationType.OcMultiReq;
            if (safe.Equals("uca", StringComparison.OrdinalIgnoreCase))
                return NotificationType.Uca;
            if (safe.Equals("fon-req", StringComparison.OrdinalIgnoreCase))
                return NotificationType.FonReq;
            if (safe.Equals("foc-req", StringComparison.OrdinalIgnoreCase))
                return NotificationType.FocReq;
            if (safe.Equals("ou-req", StringComparison.OrdinalIgnoreCase))
                return NotificationType.OuReq;
            if (safe.Equals("wallet_transfer", StringComparison.OrdinalIgnoreCase))
                return NotificationType.WalletTransfer;
            if (safe.Equals("pos_close", StringComparison.OrdinalIgnoreCase))
                return NotificationType.PosClose;
            if (safe.Equals("deposit_new", StringComparison.OrdinalIgnoreCase))
                return NotificationType.DepositNew;
            if (safe.Equals("deposit_complete", StringComparison.OrdinalIgnoreCase))
                return NotificationType.DepositComplete;

            return NotificationType.Undefined;
        }
    }
}
