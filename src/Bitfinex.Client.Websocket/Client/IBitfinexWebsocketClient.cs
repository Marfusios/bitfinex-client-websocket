using System;

namespace Bitfinex.Client.Websocket.Client;

/// <summary>
/// Bitfinex websocket client.
/// </summary>
public interface IBitfinexWebsocketClient : IDisposable
{
    /// <summary>
    /// Serializes request and sends message via websocket client. 
    /// It logs and re-throws every exception. 
    /// </summary>
    /// <param name="request">Request/message to be sent</param>
    void Send<T>(T request);
}