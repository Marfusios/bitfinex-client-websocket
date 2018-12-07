using System;
using System.Net.WebSockets;
using Bitfinex.Client.Websocket.Communicator;
using Websocket.Client;

namespace Bitfinex.Client.Websocket.Websockets
{
    /// <inheritdoc cref="WebsocketClient" />
    public class BitfinexWebsocketCommunicator : WebsocketClient, IBitfinexCommunicator
    {
        /// <inheritdoc />
        public BitfinexWebsocketCommunicator(Uri url, Func<ClientWebSocket> clientFactory = null) 
            : base(url, clientFactory)
        {
        }
    }
}
