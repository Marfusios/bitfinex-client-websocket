using System;
using System.Net.WebSockets;
using Bitfinex.Client.Websocket.Communicator;
using Microsoft.Extensions.Logging;
using Websocket.Client;

namespace Bitfinex.Client.Websocket.Websockets
{
    /// <inheritdoc cref="WebsocketClient" />
    public class BitfinexWebsocketCommunicator : WebsocketClient, IBitfinexCommunicator
    {
        /// <inheritdoc />
        public BitfinexWebsocketCommunicator(Uri url, Func<ClientWebSocket>? clientFactory = null)
            : base(url, clientFactory)
        {
        }

        /// <inheritdoc />
        public BitfinexWebsocketCommunicator(Uri url, ILogger<BitfinexWebsocketCommunicator> logger, Func<ClientWebSocket>? clientFactory = null)
            : base(url, logger, clientFactory)
        {
        }
    }
}
