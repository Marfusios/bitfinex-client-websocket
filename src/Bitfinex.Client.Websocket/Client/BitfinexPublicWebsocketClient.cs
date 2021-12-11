using System;
using System.Collections.Generic;
using Bitfinex.Client.Websocket.Json;
using Bitfinex.Client.Websocket.Messages;
using Bitfinex.Client.Websocket.Responses;
using Bitfinex.Client.Websocket.Responses.Books;
using Bitfinex.Client.Websocket.Responses.Candles;
using Bitfinex.Client.Websocket.Responses.Configurations;
using Bitfinex.Client.Websocket.Responses.FundingTrades;
using Bitfinex.Client.Websocket.Responses.Status;
using Bitfinex.Client.Websocket.Responses.Tickers;
using Bitfinex.Client.Websocket.Responses.Trades;
using Bitfinex.Client.Websocket.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Websocket.Client;

namespace Bitfinex.Client.Websocket.Client;

/// <summary>
/// Bitfinex public websocket client, it wraps `IWebsocketClient` and parse raw data into streams.
/// Send subscription requests (for example: `new TradesSubscribeRequest(pair)`) and subscribe to `Streams`.
/// </summary>
public class BitfinexPublicWebsocketClient : BitfinexWebsocketClient<BitfinexPublicClientStreams>, IBitfinexPublicWebsocketClient
{
    readonly Dictionary<int, Action<JToken>> _channelIdToHandler = new();

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="logger">The logger to use for logging any warnings or errors.</param>
    /// <param name="client">The client to use for the trade websocket.</param>
    public BitfinexPublicWebsocketClient(ILogger logger, IWebsocketClient client) : base(logger, client)
    {
        BitfinexJsonSerializer.PublicLogger = logger;
    }

    /// <inheritdoc />
    protected override string LogMessage(string message) => BitfinexLogMessage.Public(message);

    /// <inheritdoc />
    protected override void HandleObjectMessage(string message)
    {
        var parsed = BitfinexSerialization.Deserialize<MessageBase>(message);

        switch (parsed.Event)
        {
            case MessageType.Pong:
                PongResponse.Handle(message, Streams.PongStream);
                break;
            case MessageType.Error:
                ErrorResponse.Handle(message, LogError, Streams.ErrorStream);
                break;
            case MessageType.Info:
                InfoResponse.Handle(message, Streams.InfoStream);
                break;
            case MessageType.Conf:
                ConfigurationResponse.Handle(message, Streams.ConfigurationStream);
                break;
            case MessageType.Subscribed:
                OnSubscription(BitfinexSerialization.Deserialize<SubscribedResponse>(message));
                break;
            case MessageType.Unsubscribed:
                UnsubscribedResponse.Handle(message, Streams.UnsubscriptionStream);
                break;
            default:
                LogWarning($"Missing handler for public stream '{parsed.Event}'. Data: '{message}'");
                break;
        }
    }

    /// <inheritdoc />
    protected override void HandleArrayMessage(JArray message)
    {
        var channelId = (int)message[0];

        if (!_channelIdToHandler.ContainsKey(channelId))
        {
            Logger.LogDebug(LogMessage($"Unrecognized channel id '{channelId}', ignoring.."));
            return;
        }

        _channelIdToHandler[channelId](message);
    }

    void OnSubscription(SubscribedResponse response)
    {
        var channelId = response.ChanId;

        switch (response.Channel)
        {
            case "ticker":
                _channelIdToHandler[channelId] = x => Ticker.Handle(x, LogWarning, response, Configuration, Streams.TickerStream);
                break;
            case "trades":
                if (response.Pair == null) // If pair is null means that is funding
                    _channelIdToHandler[channelId] = x => FundingTrade.Handle(x, LogWarning, response, Configuration, Streams.FundingTradesStream);
                else
                    _channelIdToHandler[channelId] = x => Trade.Handle(x, LogWarning, response, Configuration, Streams.TradesStream, Streams.TradesSnapshotStream);
                break;
            case "candles":
                _channelIdToHandler[channelId] = x => Candles.Handle(x, response, Streams.CandlesStream);
                break;
            case "book":
                if ("R0".Equals(response.Prec, StringComparison.OrdinalIgnoreCase))
                    _channelIdToHandler[channelId] = x => RawBook.Handle(x, LogWarning, response, Configuration, Streams.RawBookStream, Streams.RawBookSnapshotStream, Streams.BookChecksumStream);
                else
                    _channelIdToHandler[channelId] = x => Book.Handle(x, LogWarning, response, Configuration, Streams.BookStream, Streams.BookSnapshotStream, Streams.BookChecksumStream);
                break;
            case "status":
                if (response.Key.StartsWith("deriv"))
                    _channelIdToHandler[channelId] = x => DerivativePairStatus.Handle(x, response, Streams.DerivativePairStream);
                else if (response.Key.StartsWith("liq"))
                    _channelIdToHandler[channelId] = x => LiquidationFeedStatus.Handle(x, response, Streams.LiquidationFeedStream);
                break;
            default:
                LogWarning($"Missing subscription handler '{response.Channel}'");
                break;
        }

        Streams.SubscriptionStream.OnNext(response);
    }
}