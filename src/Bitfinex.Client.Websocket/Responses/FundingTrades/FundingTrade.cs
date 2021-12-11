using Bitfinex.Client.Websocket.Json;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Responses.Configurations;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.FundingTrades;

/// <summary>
/// The order that causes the trade determines if it is a buy or a sell.
/// </summary>
[JsonConverter(typeof(FundingTradeConverter))]
public class FundingTrade : ResponseBase
{
    /// <summary>
    /// Trade id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Millisecond time stamp
    /// </summary>
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime Mts { get; set; }

    /// <summary>
    /// How much was bought (positive) or sold (negative).
    /// </summary>
    public double Amount { get; set; }

    /// <summary>
    /// Rate at which funding transaction occurred
    /// </summary>
    public double Rate { get; set; }

    /// <summary>
    /// Amount of time the funding transaction was for
    /// </summary>
    public double Period { get; set; }

    /// <summary>
    /// Type of the trade
    /// </summary>
    [JsonIgnore]
    public TradeType Type { get; set; }

    /// <summary>
    /// Target Symbol
    /// </summary>        
    [JsonIgnore]
    public string Symbol { get; set; }

    /// <summary>
    /// Target currency
    /// </summary>
    [JsonIgnore]
    public string Currency => BitfinexSymbolUtils.ExtractFundingCurrency(Symbol);

    internal static void Handle(JToken token, Action<string> logWarning, SubscribedResponse subscription,
        ConfigurationState config, Subject<FundingTrade> subject)
    {
        var firstPosition = token[1];
        if (firstPosition.Type == JTokenType.Array)
        {
            // initial snapshot
            Handle(token, logWarning, firstPosition.ToObject<FundingTrade[]>(), subscription, config, subject);
            return;
        }

        var fundingType = TradeType.Executed;
        if (firstPosition.Type == JTokenType.String)
        {
            if ((string)firstPosition == "ftu")
                fundingType = TradeType.UpdateExecution;
            else if ((string)firstPosition == "hb")
                return; // heartbeat, ignore
        }

        var data = token[2];
        if (data.Type != JTokenType.Array)
        {
            // bad format, ignore
            return;
        }

        var fundingTrade = data.ToObject<FundingTrade>();
        fundingTrade.Type = fundingType;
        fundingTrade.Symbol = subscription.Symbol;
        fundingTrade.ChanId = subscription.ChanId;
        SetGlobalData(fundingTrade, logWarning, config, token, 2);
        subject.OnNext(fundingTrade);
    }

    internal static void Handle(JToken token, Action<string> logWarning, FundingTrade[] fundings, SubscribedResponse subscription,
        ConfigurationState config, Subject<FundingTrade> subject)
    {
        var reversed = fundings.Reverse().ToArray(); // newest last
        foreach (var funding in reversed)
        {
            funding.Type = TradeType.Executed;
            funding.Symbol = subscription.Symbol;
            funding.ChanId = subscription.ChanId;
            SetGlobalData(funding, logWarning, config, token);
            subject.OnNext(funding);
        }
    }

}