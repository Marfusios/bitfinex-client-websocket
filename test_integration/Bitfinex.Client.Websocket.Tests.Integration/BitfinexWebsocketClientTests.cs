using System;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Client.Websocket.Client;
using Bitfinex.Client.Websocket.Requests;
using Bitfinex.Client.Websocket.Responses;
using Bitfinex.Client.Websocket.Websockets;
using Xunit;

namespace Bitfinex.Client.Websocket.Tests.Integration
{
    public class BitfinexWebsocketClientTests
    {
        [Fact]
        public async Task PingPong()
        {
            var url = BitfinexValues.ApiWebsocketUrl;
            using (var communicator = new BitfinexWebsocketCommunicator(url))
            {
                PongResponse received = null;
                var receivedEvent = new ManualResetEvent(false);

                using (var client = new BitfinexWebsocketClient(communicator))
                {

                    client.PongReceived.Subscribe(pong =>
                    {
                        received = pong;
                        receivedEvent.Set();
                    });

                    await communicator.Start();

                    await client.Send(new PingRequest() {Cid = 123456});

                    receivedEvent.WaitOne(TimeSpan.FromSeconds(30));

                    Assert.NotNull(received);
                    Assert.Equal(123456, received.Cid);
                }
            }
        }

    }
}
