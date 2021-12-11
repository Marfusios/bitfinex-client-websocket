using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Responses;
using Bitfinex.Client.Websocket.Responses.Configurations;

namespace Bitfinex.Client.Websocket.Client;

/// <summary>
/// Base class for streams from Bitfinex websocket API.
/// </summary>
public abstract class BitfinexClientStreams
{
    /// <summary>
    /// Info about every occurred error
    /// </summary>
    public readonly Subject<ErrorResponse> ErrorStream = new();

    /// <summary>
    /// Initial info stream, publishes always on a new connection
    /// </summary>
    public readonly Subject<InfoResponse> InfoStream = new();

    /// <summary>
    /// Pong stream to match every ping request
    /// </summary>
    public readonly Subject<PongResponse> PongStream = new();

    /// <summary>
    /// Info about processed configuration
    /// </summary>
    public readonly Subject<ConfigurationResponse> ConfigurationStream = new();
}