using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Responses;
using Bitfinex.Client.Websocket.Responses.Balance;
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

namespace Bitfinex.Client.Websocket.Client;

/// <summary>
/// Authenticated websocket streams.
/// </summary>
public class BitfinexAuthenticatedClientStreams : BitfinexClientStreams
{
    /// <summary>
    /// Info about processed authentication
    /// </summary>
    public readonly Subject<AuthenticationResponse> AuthenticationStream = new();

    /// <summary>
    /// Notifications
    /// </summary>
    public readonly Subject<Notification> NotificationStream = new();

    /// <summary>
    /// Private initial info about all wallets/balances (streamed only on authentication)
    /// </summary>
    public readonly Subject<Wallet[]> WalletsStream = new();

    /// <summary>
    /// Private stream for every wallet balance update (initial wallets info is also streamed, same as 'WalletsStream')
    /// </summary>
    public readonly Subject<Wallet> WalletStream = new();

    /// <summary>
    /// Private info about executed trades
    /// </summary>
    public readonly Subject<PrivateTrade> PrivateTradesStream = new();

    /// <summary>
    /// Private info about executed funding trades
    /// </summary>
    public readonly Subject<PrivateFundingTrade> PrivateFundingTradesStream = new();

    /// <summary>
    /// Private initial info about all opened orders (streamed only on authentication)
    /// </summary>
    public readonly Subject<Order[]> OrdersStream = new();

    /// <summary>
    /// Private info about created/placed order
    /// </summary>
    public readonly Subject<Order> OrderCreatedStream = new();

    /// <summary>
    /// Private info about updated order
    /// </summary>
    public readonly Subject<Order> OrderUpdatedStream = new();

    /// <summary>
    /// Private info about canceled or executed order
    /// </summary>
    public readonly Subject<Order> OrderCanceledStream = new();

    /// <summary>
    /// Private initial info about all opened funding offers (streamed only on authentication)
    /// </summary>
    public readonly Subject<FundingOffer[]> FundingOffersStream = new();

    /// <summary>
    /// Private info about created/placed funding offer
    /// </summary>
    public readonly Subject<FundingOffer> FundingOfferCreatedStream = new();

    /// <summary>
    /// Private info about updated funding offer
    /// </summary>
    public readonly Subject<FundingOffer> FundingOfferUpdatedStream = new();

    /// <summary>
    /// Private info about canceled or executed funding offer
    /// </summary>
    public readonly Subject<FundingOffer> FundingOfferCanceledStream = new();

    /// <summary>
    /// Private initial info about all opened funding credits (streamed only on authentication)
    /// </summary>
    public readonly Subject<FundingCredit[]> FundingCreditsStream = new();

    /// <summary>
    /// Private info about created/placed funding credit
    /// </summary>
    public readonly Subject<FundingCredit> FundingCreditCreatedStream = new();

    /// <summary>
    /// Private info about updated funding credit
    /// </summary>
    public readonly Subject<FundingCredit> FundingCreditUpdatedStream = new();

    /// <summary>
    /// Private info about canceled or executed funding credit
    /// </summary>
    public readonly Subject<FundingCredit> FundingCreditCanceledStream = new();

    /// <summary>
    /// Private initial info about all opened funding loans (streamed only on authentication)
    /// </summary>
    public readonly Subject<FundingLoan[]> FundingLoansStream = new();

    /// <summary>
    /// Private info about created/placed funding loan
    /// </summary>
    public readonly Subject<FundingLoan> FundingLoanCreatedStream = new();

    /// <summary>
    /// Private info about updated funding loan
    /// </summary>
    public readonly Subject<FundingLoan> FundingLoanUpdatedStream = new();

    /// <summary>
    /// Private info about canceled or executed funding loan
    /// </summary>
    public readonly Subject<FundingLoan> FundingLoanCanceledStream = new();

    /// <summary>
    /// Private initial info about all opened positions (streamed only on authentication)
    /// </summary>
    public readonly Subject<Position[]> PositionsStream = new();

    /// <summary>
    /// Private info about created/opened position
    /// </summary>
    public readonly Subject<Position> PositionCreatedStream = new();

    /// <summary>
    /// Private info about updated position
    /// </summary>
    public readonly Subject<Position> PositionUpdatedStream = new();

    /// <summary>
    /// Private info about canceled or closed position
    /// </summary>
    public readonly Subject<Position> PositionCanceledStream = new();

    /// <summary>
    /// Private info about total balances
    /// </summary>
    public readonly Subject<BalanceInfo> BalanceInfoStream = new();

    /// <summary>
    /// Private info about margin data
    /// </summary>
    public readonly Subject<MarginInfo> MarginInfoStream = new();
}