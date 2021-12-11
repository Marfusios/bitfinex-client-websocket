using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Balance;

/// <summary>
/// Balance response
/// </summary>
[DebuggerDisplay("BalanceInfo {TotalAum} {NetAum}")]
[JsonConverter(typeof(BalanceInfoConverter))]
public class BalanceInfo
{
    //static readonly ILog Log = LogProvider.GetCurrentClassLogger();

    /// <summary>
    /// Total Assets Under Management
    /// </summary>
    public double TotalAum { get; set; }

    /// <summary>
    /// Net Assets Under Management
    /// </summary>
    public double NetAum { get; set; }

    internal static void Handle(JToken token, Action<string> logWarning, Subject<BalanceInfo> subject)
    {
        var data = token[2];
        if (data.Type != JTokenType.Array)
        {
            logWarning("BalanceInfo - Invalid message format, third param not array");
            return;
        }

        var parsed = data.ToObject<BalanceInfo>();
        subject.OnNext(parsed);
    }
}