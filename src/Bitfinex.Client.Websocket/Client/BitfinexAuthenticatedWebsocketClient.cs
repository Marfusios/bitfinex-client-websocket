using Bitfinex.Client.Websocket.Json;
using Bitfinex.Client.Websocket.Messages;
using Bitfinex.Client.Websocket.Responses;
using Bitfinex.Client.Websocket.Responses.Balance;
using Bitfinex.Client.Websocket.Responses.Configurations;
using Bitfinex.Client.Websocket.Responses.FundingCredits;
using Bitfinex.Client.Websocket.Responses.FundingLoans;
using Bitfinex.Client.Websocket.Responses.FundingOffers;
using Bitfinex.Client.Websocket.Responses.FundingTradesPrivate;
using Bitfinex.Client.Websocket.Responses.Margin;
using Bitfinex.Client.Websocket.Responses.Notifications;
using Bitfinex.Client.Websocket.Responses.Orders;
using Bitfinex.Client.Websocket.Responses.Positions;
using Bitfinex.Client.Websocket.Responses.TradesPrivate;
using Bitfinex.Client.Websocket.Responses.Wallets;
using Bitfinex.Client.Websocket.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Websocket.Client;

namespace Bitfinex.Client.Websocket.Client;

/// <summary>
/// Bitfinex authenticated websocket client, it wraps `IWebsocketClient` and parse raw data into streams.
/// </summary>
public class BitfinexAuthenticatedWebsocketClient : BitfinexWebsocketClient<BitfinexAuthenticatedClientStreams>, IBitfinexAuthenticatedWebsocketClient
{
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="logger">The logger to use for logging any warnings or errors.</param>
    /// <param name="client">The client to use for the trade websocket.</param>
    public BitfinexAuthenticatedWebsocketClient(ILogger logger, IWebsocketClient client) : base(logger, client)
    {
        BitfinexJsonSerializer.AuthenticatedLogger = logger;
    }

    /// <inheritdoc />
    protected override string LogMessage(string message) => BitfinexLogMessage.Authenticated(message);

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
            case MessageType.Auth:
                AuthenticationResponse.Handle(message, LogWarning, Streams.AuthenticationStream);
                break;
            case MessageType.Conf:
                ConfigurationResponse.Handle(message, Streams.ConfigurationStream);
                break;
            default:
                LogWarning($"Missing handler for authenticated stream '{parsed.Event}'. Data: '{message}'");
                break;
        }
    }

    /// <inheritdoc />
    protected override void HandleArrayMessage(JArray message)
    {
        var secondItem = message[1];
        if (secondItem.Type != JTokenType.String)
        {
            LogWarning("Invalid message format, second param is not string");
            return;
        }
        var msgType = (string)secondItem;
        if (msgType == "hb")
        {
            // heartbeat, ignore
            return;
        }

        if (message.Count < 3)
        {
            LogWarning("Invalid message format, too few items");
            return;
        }

        switch (msgType)
        {
            case "ws":
                Wallet.Handle(message, LogWarning, Streams.WalletStream, Streams.WalletsStream);
                break;
            case "wu":
                Wallet.Handle(message, LogWarning, Streams.WalletStream);
                break;
            case "os":
                Order.Handle(message, LogWarning, Streams.OrdersStream);
                break;
            case "on":
                Order.Handle(message, LogWarning, Streams.OrderCreatedStream);
                break;
            case "ou":
                Order.Handle(message, LogWarning, Streams.OrderUpdatedStream);
                break;
            case "oc":
                Order.Handle(message, LogWarning, Streams.OrderCanceledStream);
                break;
            case "fos":
                FundingOffer.Handle(message, LogWarning, Streams.FundingOffersStream);
                break;
            case "fon":
                FundingOffer.Handle(message, LogWarning, Streams.FundingOfferCreatedStream);
                break;
            case "fou":
                FundingOffer.Handle(message, LogWarning, Streams.FundingOfferUpdatedStream);
                break;
            case "foc":
                FundingOffer.Handle(message, LogWarning, Streams.FundingOfferCanceledStream);
                break;
            case "fcs":
                FundingCredit.Handle(message, LogWarning, Streams.FundingCreditsStream);
                break;
            case "fcn":
                FundingCredit.Handle(message, LogWarning, Streams.FundingCreditCreatedStream);
                break;
            case "fcu":
                FundingCredit.Handle(message, LogWarning, Streams.FundingCreditUpdatedStream);
                break;
            case "fcc":
                FundingCredit.Handle(message, LogWarning, Streams.FundingCreditCanceledStream);
                break;
            case "fls":
                FundingLoan.Handle(message, LogWarning, Streams.FundingLoansStream);
                break;
            case "fln":
                FundingLoan.Handle(message, LogWarning, Streams.FundingLoanCreatedStream);
                break;
            case "flu":
                FundingLoan.Handle(message, LogWarning, Streams.FundingLoanUpdatedStream);
                break;
            case "flc":
                FundingLoan.Handle(message, LogWarning, Streams.FundingLoanCanceledStream);
                break;
            case "te":
                PrivateTrade.Handle(message, LogWarning, Configuration, Streams.PrivateTradesStream, TradeType.Executed);
                break;
            case "tu":
                PrivateTrade.Handle(message, LogWarning, Configuration, Streams.PrivateTradesStream, TradeType.UpdateExecution);
                break;
            case "fte":
                PrivateFundingTrade.Handle(message, LogWarning, Configuration, Streams.PrivateFundingTradesStream, TradeType.Executed);
                break;
            case "ftu":
                PrivateFundingTrade.Handle(message, LogWarning, Configuration, Streams.PrivateFundingTradesStream, TradeType.UpdateExecution);
                break;
            case "ps":
                Position.Handle(message, LogWarning, Configuration, Streams.PositionsStream);
                break;
            case "pn":
                Position.Handle(message, LogWarning, Configuration, Streams.PositionCreatedStream);
                break;
            case "pu":
                Position.Handle(message, LogWarning, Configuration, Streams.PositionUpdatedStream);
                break;
            case "pc":
                Position.Handle(message, LogWarning, Configuration, Streams.PositionCanceledStream);
                break;
            case "n":
                Notification.Handle(message, LogWarning, Streams.NotificationStream);
                break;
            case "bu":
                BalanceInfo.Handle(message, LogWarning, Streams.BalanceInfoStream);
                break;
            case "miu":
                MarginInfo.Handle(message, LogWarning, Streams.MarginInfoStream);
                break;
            default:
                LogWarning($"Missing handler for authenticated stream '{msgType}'. Data: {message}");
                break;
        }
    }
}