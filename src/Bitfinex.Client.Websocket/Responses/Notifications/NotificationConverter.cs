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
            var safe = type.ToLower().Trim();
            switch (safe)
            {
                case "on-req":
                    return NotificationType.OnReq;
                case "oc-req":
                    return NotificationType.OcReq;
                case "on_multi-req":
                    return NotificationType.OnMultiReq;
                case "oc_multi-req":
                    return NotificationType.OcMultiReq;
                case "uca":
                    return NotificationType.Uca;
                case "fon-req":
                    return NotificationType.FonReq;
                case "foc-req":
                    return NotificationType.FocReq;
                case "ou-req":
                    return NotificationType.OuReq;
                case "wallet_transfer":
                    return NotificationType.WalletTransfer;
                case "pos_close":
                    return NotificationType.PosClose;
                case "deposit_new":
                    return NotificationType.DepositNew;
                case "deposit_complete":
                    return NotificationType.DepositComplete;
            }

            return NotificationType.Undefined;
        }
    }
}