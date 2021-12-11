namespace Bitfinex.Client.Websocket.Responses.Positions;

/// <summary>
/// Status  of the position
/// </summary>
public enum PositionStatus
{
    /// <summary>
    /// Not known status (failed to parse or received null)
    /// </summary>
    Undefined,

    /// <summary>
    /// Position is active
    /// </summary>
    Active,

    /// <summary>
    /// Position is closed
    /// </summary>
    Closed
}