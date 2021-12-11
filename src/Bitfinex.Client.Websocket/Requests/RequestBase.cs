using Bitfinex.Client.Websocket.Messages;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests;

/// <summary>
/// Base class for every request
/// </summary>
public abstract class RequestBase : MessageBase
{
    /// <inheritdoc />
    public override MessageType Event
    {
        get => EventType;
        set { }
    }

    /// <summary>
    /// Unique event type, need to be set in descendants
    /// </summary>
    [JsonIgnore]
    public abstract MessageType EventType { get; }
}