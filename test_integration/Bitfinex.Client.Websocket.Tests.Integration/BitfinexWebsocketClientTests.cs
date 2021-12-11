﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Client.Websocket.Client;
using Bitfinex.Client.Websocket.Requests;
using Bitfinex.Client.Websocket.Responses;
using Microsoft.Extensions.Logging.Abstractions;
using Websocket.Client;
using Xunit;

namespace Bitfinex.Client.Websocket.Tests.Integration;

public class BitfinexWebsocketClientTests
{
    static readonly string API_KEY = "your_api_key";
    static readonly string API_SECRET = "";

    [Fact]
    public async Task PingPong()
    {
        var url = BitfinexValues.BitfinexPublicWebsocketUrl;
        using var apiClient = new WebsocketClient(url);
        PongResponse received = null;
        var receivedEvent = new ManualResetEvent(false);

        using var client = new BitfinexPublicWebsocketClient(NullLogger.Instance, apiClient);
        client.Streams.PongStream.Subscribe(pong =>
        {
            received = pong;
            receivedEvent.Set();
        });

        await apiClient.Start();

        client.Send(new PingRequest {Cid = 123456});

        receivedEvent.WaitOne(TimeSpan.FromSeconds(30));

        Assert.NotNull(received);
        Assert.Equal(123456, received.Cid);
        Assert.True(DateTime.UtcNow.Subtract(received.Ts).TotalSeconds < 15);
    }

    [SkippableFact]
    public async Task Authentication()
    {
        Skip.If(string.IsNullOrWhiteSpace(API_SECRET));

        var url = BitfinexValues.ApiWebsocketUrl;
        using var apiClient = new WebsocketClient(url);
        AuthenticationResponse received = null;
        var receivedEvent = new ManualResetEvent(false);

        using var client = new BitfinexAuthenticatedWebsocketClient(NullLogger.Instance, apiClient);
        client.Streams.AuthenticationStream.Subscribe(auth =>
        {
            received = auth;
            receivedEvent.Set();
        });

        await apiClient.Start();

        client.Send(new AuthenticationRequest(API_KEY, API_SECRET));

        receivedEvent.WaitOne(TimeSpan.FromSeconds(30));

        Assert.NotNull(received);
        Assert.True(received.IsAuthenticated);
    }

}