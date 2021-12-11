using Bitfinex.Client.Websocket.Json;
using Newtonsoft.Json;
using System;
using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Responses.Configurations;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.FundingTradesPrivate;

/// <summary>
/// The order that causes the trade determines if it is a buy or a sell.
/// </summary>
[JsonConverter(typeof(PrivateFundingTradeConverter))]
public class PrivateFundingTrade : ResponseBase
{
    /// <summary>
    /// Trade id
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Symbol (fUSD, etc)
    /// </summary>
    public string Symbol { get; set; }
    
    /// <summary>
    /// Execution timestamp
    /// </summary>
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime MtsCreate { get; set; }

    /// <summary>
    /// Offer id
    /// </summary>
    public long OfferId { get; set; }

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
    /// True if maker order (post-only)
    /// </summary>
    public bool IsMaker { get; set; }

    /// <summary>
    /// Type of the trade
    /// </summary>
    [JsonIgnore]
    public TradeType Type { get; set; }

    /// <summary>
    /// Target currency
    /// </summary>
    [JsonIgnore]
    public string Currency => BitfinexSymbolUtils.ExtractFundingCurrency(Symbol);

    internal static void Handle(JToken token, Action<string> logWarning, ConfigurationState config, Subject<PrivateFundingTrade> subject, TradeType type)
    {
        var data = token[2];
        if (data.Type != JTokenType.Array)
        {
            logWarning("Private funding trade info - Invalid message format, third param not array");
            return;
        }

        var fundingTrade = data.ToObject<PrivateFundingTrade>();
        fundingTrade.Type = type;
        SetGlobalData(fundingTrade, logWarning, config, token, 2, true);

        subject.OnNext(fundingTrade);
    }
}