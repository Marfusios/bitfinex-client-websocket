namespace Bitfinex.Client.Websocket.Responses;

/// <summary>
/// Current funding status
/// </summary>
public enum FundingStatus
{
    /// <summary>
    /// Unknown offer status (maybe new API)
    /// </summary>
    Undefined,

    /// <summary>
    /// Offer is active (placed in the order book)
    /// </summary>
    Active,

    /// <summary>
    /// Offer was fully executed
    /// </summary>
    Executed,

    /// <summary>
    /// Offer was only partially executed (still staying in the offer book)
    /// </summary>
    PartiallyFilled,

    /// <summary>
    /// Offer was canceled
    /// </summary>
    Canceled
}
