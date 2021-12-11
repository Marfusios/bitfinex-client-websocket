using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.FundingLoans;

/// <summary>
/// Info about funding credit
/// </summary>
[DebuggerDisplay("Credit: {Id} - {Symbol} - {Amount}")]
[JsonConverter(typeof(FundingLoanConverter))]
public class FundingLoan
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
    /// 1 if you are the lender, 0 if you are both the lender and borrower, -1 if you're the borrower
    /// </summary>
    public FundingSide Side { get; set; }

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
    /// Future params object (stay tuned)
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
    /// Period of the offer
    /// </summary>
    public double Period { get; set; }

    /// <summary>
    /// Millisecond Time Stamp when the funding was opened
    /// </summary>
    public DateTime? MtsOpening { get; set; }

    /// <summary>
    /// Millisecond Time Stamp when the last payout was received
    /// </summary>
    public DateTime? MtsLastPayout { get; set; }

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
    /// The calculated rate for FRR and FRRDELTAFIX
    /// </summary>
    public double RateReal { get; set; }

    /// <summary>
    /// Whether the funding should be closed when the position is closed
    /// </summary>
    public bool NoClose { get; set; }

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

    internal static void Handle(JToken token, Action<string> logWarning, Subject<FundingLoan[]> subject)
    {
        var data = token[2];
        if (data.Type != JTokenType.Array)
        {
            logWarning("Funding Loans - Invalid message format, third param not array");
            return;
        }

        var parsed = data.ToObject<FundingLoan[]>();
        subject.OnNext(parsed);
    }

    internal static void Handle(JToken token, Action<string> logWarning, Subject<FundingLoan> subject)
    {
        var data = token[2];
        if (data.Type != JTokenType.Array)
        {
            logWarning("Funding Loan - Invalid message format, third param not array");
            return;
        }

        var parsed = data.ToObject<FundingLoan>();

        subject.OnNext(parsed);
    }
}