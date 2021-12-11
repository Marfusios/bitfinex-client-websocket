namespace Bitfinex.Client.Websocket.Client;

/// <summary>
/// Bitfinex public websocket client.
/// </summary>
public interface IBitfinexPublicWebsocketClient : IBitfinexWebsocketClient
{
    /// <summary>
    /// Provided message streams.
    /// </summary>
    BitfinexPublicClientStreams Streams { get; }
}