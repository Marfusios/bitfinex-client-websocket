namespace Bitfinex.Client.Websocket.Responses.Positions;

/// <summary>
/// Type of the margin funding (borrowed assets/money)
/// </summary>
public enum MarginFundingType
{
    /// <summary>
    /// Not known type (failed to parse or received null)
    /// </summary>
    Undefined,

    /// <summary>
    /// Funding is taken daily
    /// </summary>
    Daily,

    /// <summary>
    /// Funding is taken at the end of term
    /// </summary>
    Term
}