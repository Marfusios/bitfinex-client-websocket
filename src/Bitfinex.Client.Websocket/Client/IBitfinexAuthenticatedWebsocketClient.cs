namespace Bitfinex.Client.Websocket.Client;

/// <summary>
/// Bitfinex authenticated websocket client.
/// </summary>
public interface IBitfinexAuthenticatedWebsocketClient : IBitfinexWebsocketClient
{
    /// <summary>
    /// Provided message streams.
    /// </summary>
    BitfinexAuthenticatedClientStreams Streams { get; }
}