namespace Bitfinex.Client.Websocket.Responses.FundingOffers;

/// <summary>
/// Type of the offer
/// </summary>
public enum FundingOfferType
{
    /// <summary>
    /// Unknown
    /// </summary>
    Undefined,

    /// <summary>
    /// Limit (maker) offer.
    /// </summary>
    Limit,

    /// <summary>
    /// Flash return rate delta variable offer.
    /// </summary>
    FrrDeltaVar
}