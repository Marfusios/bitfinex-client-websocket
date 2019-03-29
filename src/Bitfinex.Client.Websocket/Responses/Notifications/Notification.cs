using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Notifications
{
    [DebuggerDisplay("Notification: {Status} - {Text}")]
    [JsonConverter(typeof(NotificationConverter))]
    public class Notification
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        public DateTime Mts { get; set; }
        public NotificationType Type { get; set; }
        public long? MessageId { get; set; }
        public string NotifyInfo { get; set; }
        public long? Code { get; set; }
        public string Status { get; set; }
        public string Text { get; set; }

        public static void Handle(JToken token, Subject<Notification> subject)
        {
            var data = token[2];
            if (data.Type != JTokenType.Array)
            {
                Log.Warn(L("Notification - Invalid message format, third param not array"));
                return;
            }

            var parsed = data.ToObject<Notification>();
            subject.OnNext(parsed);
        }

        private static string L(string msg)
        {
            return $"[BFX NOTIFICATION HANDLER] {msg}";
        }
    }
}