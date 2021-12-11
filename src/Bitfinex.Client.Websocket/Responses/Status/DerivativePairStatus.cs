using System;
using System.Reactive.Subjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Status;

/// <summary>
/// Derivative pair status
/// </summary>
[JsonConverter(typeof(DerivativePairStatusConverter))]
public class DerivativePairStatus : ResponseBase
{
    /// <summary>
    /// Millisecond timestamp
    /// </summary>
    public DateTime TimestampMs { get; set; }

    /// <summary>
    /// Derivative last traded price.
    /// </summary>
    public double DerivPrice { get; set; }

    /// <summary>
    /// Last traded price of the underlying Bitfinex spot trading pair
    /// </summary>
    public double SpotPrice { get; set; }

    /// <summary>
    /// The balance available to the liquidation engine to absorb losses.
    /// </summary>
    public double InsuranceFundBalance { get; set; }

    /// <summary>
    /// ?
    /// </summary>
    public double FundingAccrued { get; set; }

    /// <summary>
    /// Derivative symbol
    /// </summary>
    public string Symbol { get; set; }

    /// <summary>
    /// ?
    /// </summary>
    public long FundingStep { get; set; }

    internal static void Handle(JToken token, SubscribedResponse subscription,
        Subject<DerivativePairStatus> subject)
    {
        var data = token[1];

        if (data.Type != JTokenType.Array)
        {
            // probably heartbeat, ignore
            return;
        }

        var derivativePairStatus = data.ToObject<DerivativePairStatus>();
        derivativePairStatus.Symbol = subscription.Key.Substring(subscription.Key.IndexOf(':') + 1);
        derivativePairStatus.ChanId = subscription.ChanId;

        subject.OnNext(derivativePairStatus);
    }
}