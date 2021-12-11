using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Notifications;

/// <summary>
/// Notification response
/// </summary>
[DebuggerDisplay("Notification: {Status} - {Text}")]
[JsonConverter(typeof(NotificationConverter))]
public class Notification
{
    /// <summary>
    /// Timestamp of the update
    /// </summary>
    public DateTime Mts { get; set; }

    /// <summary>
    /// Purpose of the notification
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// An unique ID of the message
    /// </summary>
    public long? MessageId { get; set; }

    /// <summary>
    /// A message containing information regarding the notification
    /// </summary>
    public string NotifyInfo { get; set; }

    /// <summary>
    /// Work in progress
    /// </summary>
    public long? Code { get; set; }

    /// <summary>
    /// Status of the notification; it may vary over time (SUCCESS, ERROR, FAILURE, ...)
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Text of the notification
    /// </summary>
    public string Text { get; set; }


    internal static void Handle(JToken token, Action<string> logWarning, Subject<Notification> subject)
    {
        var data = token[2];
        if (data.Type != JTokenType.Array)
        {
            logWarning("Notification - Invalid message format, third param not array");
            return;
        }

        var parsed = data.ToObject<Notification>();
        subject.OnNext(parsed);
    }
}