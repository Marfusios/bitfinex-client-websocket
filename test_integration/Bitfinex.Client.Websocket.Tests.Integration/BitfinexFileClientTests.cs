using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bitfinex.Client.Websocket.Client;
using Bitfinex.Client.Websocket.Files;
using Bitfinex.Client.Websocket.Responses.Trades;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Bitfinex.Client.Websocket.Tests.Integration;

public class BitfinexFileClientTests
{
    // ----------------------------------------------------------------
    // Don't forget to decompress gzip files before starting the tests
    // ----------------------------------------------------------------

    [SkippableFact]
    public async Task OnStart_ShouldStreamMessagesFromFile()
    {
        var files = new[]
        {
            "data/bitfinex_raw_2018-11-12.txt"
        };
        foreach (var file in files)
        {
            var exist = File.Exists(file);
            Skip.If(!exist, $"The file '{file}' doesn't exist. Don't forget to decompress gzip file!");
        }

        var trades = new List<Trade>();

        var fileClient = new BitfinexFileClient
        {
            FileNames = files,
            Delimiter = ";;"
        };

        var client = new BitfinexPublicWebsocketClient(NullLogger.Instance, fileClient);
        client.Streams.TradesStream.Subscribe(trade =>
        {
            trades.Add(trade);
        });

        await fileClient.Start();

        Assert.Equal(8938, trades.Count);
    }
}