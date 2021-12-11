namespace Bitfinex.Client.Websocket.Responses.Orders;

/// <summary>
/// Current order status
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Unknown order status (maybe new API)
    /// </summary>
    Undefined,

    /// <summary>
    /// Order is active (placed in the order book)
    /// </summary>
    Active,

    /// <summary>
    /// Order was fully executed
    /// </summary>
    Executed,

    /// <summary>
    /// Order was only partially executed (still staying in the order book)
    /// </summary>
    PartiallyFilled,

    /// <summary>
    /// Order was canceled
    /// </summary>
    Canceled,

    /// <summary>
    /// Order was canceled because it cannot be executed as maker (would be taker)
    /// </summary>
    PostOnlyCanceled,

    /// <summary>
    /// (?) Need docs
    /// </summary>
    RsnPosReduceFlip,

    /// <summary>
    /// (?) Need docs
    /// </summary>
    RsnPosReduceIncr,

    /// <summary>
    /// Order was canceled because of insufficient funds
    /// </summary>
    InsufficientBalance, 

    /// <summary>
    /// Order was canceled because of insufficient margin
    /// </summary>
    InsufficientMargin
}