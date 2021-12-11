namespace Bitfinex.Client.Websocket.Messages;

/// <summary>
/// Base class for every message
/// </summary>
public class MessageBase
{
    /// <summary>
    /// Unique message type
    /// </summary>
    public virtual MessageType Event { get; set; }
}