using System;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Client.Websocket.Websockets;
using Xunit;

namespace Bitfinex.Client.Websocket.Tests.Integration
{
    public class BitfinexWebsocketCommunicatorTests
    {
        [Fact]
        public async Task OnStarting_ShouldGetInfoResponse()
        {
            var url = BitfinexValues.ApiWebsocketUrl;
            using (var communicator = new BitfinexWebsocketCommunicator(url))
            {
                string received = null;
                var receivedEvent = new ManualResetEvent(false);

                communicator.MessageReceived.Subscribe(msg =>
                {
                    received = msg;
                    receivedEvent.Set();
                });

                await communicator.Start();

                receivedEvent.WaitOne(TimeSpan.FromSeconds(30));

                Assert.NotNull(received);
                Assert.Equal("{\"event\":\"info\",\"version\":2}", received);
            }
        }
    }
}
