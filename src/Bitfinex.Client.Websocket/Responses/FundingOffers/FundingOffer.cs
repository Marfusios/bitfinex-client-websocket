using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.FundingOffers;

/// <summary>
/// Info about placed offer
/// </summary>
[DebuggerDisplay("Offer: {Id} - {Symbol} - {Amount}")]
[JsonConverter(typeof(FundingOfferConverter))]
public class FundingOffer
{
    /// <summary>
    /// Offer ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Symbol (fUSD, …)
    /// </summary>
    public string Symbol { get; set; }

    /// <summary>
    /// Millisecond timestamp of creation
    /// </summary>
    public DateTime? MtsCreate { get; set; }

    /// <summary>
    /// Millisecond timestamp of update
    /// </summary>
    public DateTime? MtsUpdate { get; set; }

    /// <summary>
    /// Amount the offer is for
    /// </summary>
    public double? Amount { get; set; }

    /// <summary>
    /// Amount the offer was entered with originally
    /// </summary>
    public double? AmountOrig { get; set; }

    /// <summary>
    /// The type of the offer
    /// </summary>
    public FundingOfferType OfferType { get; set; }

    /// <summary>
    /// Hidden | Close | Post Only | Reduce Only | No Var Rates | OCO
    /// </summary>
    public int? Flags { get; set; }

    /// <summary>
    /// Current offer status
    /// </summary>
    public FundingStatus Status { get; set; }

    /// <summary>
    /// Rate of the offer
    /// </summary>
    public double Rate { get; set; }

    /// <summary>
    /// Amount of time the funding offer is for
    /// </summary>
    public double Period { get; set; }

    /// <summary>
    /// Should notify
    /// </summary>
    public bool Notify { get; set; }

    /// <summary>
    /// Is hidden offer
    /// </summary>
    public bool Hidden { get; set; }

    /// <summary>
    /// Should renew
    /// </summary>
    public bool Renew { get; set; }

    /// <summary>
    /// Target currency
    /// </summary>
    [JsonIgnore]
    public string Currency => BitfinexSymbolUtils.ExtractFundingCurrency(Symbol);

    /// <summary>
    /// Returns true if the <see cref="Status"/> represents an active offer (placed in the book)
    /// </summary>
    [JsonIgnore]
    public bool IsActive => Status == FundingStatus.Active ||
                            Status == FundingStatus.PartiallyFilled;

    /// <summary>
    /// Returns true if the <see cref="Status"/> represents an inactive order (canceled or executed)
    /// </summary>
    [JsonIgnore]
    public bool IsInactive => !IsActive;

    /// <summary>
    /// Returns true if the <see cref="Status"/> represents a terminated offer
    /// </summary>
    [JsonIgnore]
    public bool IsCanceled => Status == FundingStatus.Canceled ||
                              Status == FundingStatus.Undefined;

    internal static void Handle(JToken token, Action<string> logWarning, Subject<FundingOffer[]> subject)
    {
        var data = token[2];
        if (data.Type != JTokenType.Array)
        {
            logWarning("Funding Offers - Invalid message format, third param not array");
            return;
        }

        var parsed = data.ToObject<FundingOffer[]>();
        subject.OnNext(parsed);
    }

    internal static void Handle(JToken token, Action<string> logWarning, Subject<FundingOffer> subject)
    {
        var data = token[2];
        if (data.Type != JTokenType.Array)
        {
            logWarning("Funding Offer - Invalid message format, third param not array");
            return;
        }

        var parsed = data.ToObject<FundingOffer>();

        subject.OnNext(parsed);
    }
}