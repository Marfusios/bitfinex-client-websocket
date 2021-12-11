namespace Bitfinex.Client.Websocket.Responses;

/// <summary>
/// Type of the trade
/// </summary>
public enum TradeType
{
    /// <summary>
    /// Initial information (faster)
    /// </summary>
    Executed,

    /// <summary>
    /// Extended information (slower)
    /// </summary>
    UpdateExecution
}