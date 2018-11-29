using System;
using System.Net.WebSockets;
using Bitfinex.Client.Websocket.Communicator;
using Websocket.Client;

namespace Bitfinex.Client.Websocket.Websockets
{
    public class BitfinexWebsocketCommunicator : WebsocketClient, IBitfinexCommunicator
    {
        public BitfinexWebsocketCommunicator(Uri url, Func<ClientWebSocket> clientFactory = null) 
            : base(url, clientFactory)
        {
        }
    }
}
