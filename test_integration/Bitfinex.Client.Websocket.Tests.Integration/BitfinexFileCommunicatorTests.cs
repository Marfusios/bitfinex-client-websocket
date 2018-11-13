using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bitfinex.Client.Websocket.Client;
using Bitfinex.Client.Websocket.Files;
using Bitfinex.Client.Websocket.Responses.Trades;
using Xunit;

namespace Bitfinex.Client.Websocket.Tests.Integration
{
    public class BitfinexFileCommunicatorTests
    {
        // ----------------------------------------------------------------
        // Don't forget to decompress gzip files before starting the tests
        // ----------------------------------------------------------------

        [Fact]
        public async Task OnStart_ShouldStreamMessagesFromFile()
        {
            var files = new[]
            {
                "data/bitfinex_raw_2018-11-12.txt"
            };
            var trades = new List<Trade>();

            var communicator = new BitfinexFileCommunicator();
            communicator.FileNames = files;
            communicator.Delimiter = ";;";

            var client = new BitfinexWebsocketClient(communicator);
            client.Streams.TradesStream.Subscribe(trade =>
            {
                trades.Add(trade);
            });

            await communicator.Start();

            Assert.Equal(8998, trades.Count);
        }
    }
}
