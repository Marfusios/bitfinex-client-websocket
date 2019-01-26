using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Responses;
using Bitfinex.Client.Websocket.Responses.Books;
using Bitfinex.Client.Websocket.Responses.Candles;
using Bitfinex.Client.Websocket.Responses.Configurations;
using Bitfinex.Client.Websocket.Responses.Fundings;
using Bitfinex.Client.Websocket.Responses.Orders;
using Bitfinex.Client.Websocket.Responses.Tickers;
using Bitfinex.Client.Websocket.Responses.Trades;
using Bitfinex.Client.Websocket.Responses.Wallets;

namespace Bitfinex.Client.Websocket.Client
{
    /// <summary>
    /// All provided streams from Bitfinex websocket API.
    /// You need to subscribe first, send subscription request (for example: `await client.Send(new TradesSubscribeRequest(pair))`)
    /// </summary>
    public class BitfinexClientStreams
    {
        internal readonly Subject<ErrorResponse> ErrorSubject = new Subject<ErrorResponse>();
        internal readonly Subject<InfoResponse> InfoSubject = new Subject<InfoResponse>();
        internal readonly Subject<PongResponse> PongSubject = new Subject<PongResponse>();
        internal readonly Subject<AuthenticationResponse> AuthenticationSubject = new Subject<AuthenticationResponse>();
        internal readonly Subject<ConfigurationResponse> ConfigurationSubject = new Subject<ConfigurationResponse>();
        internal readonly Subject<SubscribedResponse> SubscriptionSubject = new Subject<SubscribedResponse>();
        internal readonly Subject<UnsubscribedResponse> UnsubscriptionSubject = new Subject<UnsubscribedResponse>();

        internal readonly Subject<Ticker> TickerSubject = new Subject<Ticker>();
        internal readonly Subject<Trade> TradesSubject = new Subject<Trade>();
        internal readonly Subject<Funding> FundingsSubject = new Subject<Funding>();
        internal readonly Subject<Candles> CandlesSubject = new Subject<Candles>();
        internal readonly Subject<Book> BookSubject = new Subject<Book>();
        internal readonly Subject<Book[]> BookSnapshotSubject = new Subject<Book[]>();

        internal readonly Subject<Wallet[]> WalletsSubject = new Subject<Wallet[]>();
        internal readonly Subject<Wallet> WalletSubject = new Subject<Wallet>();
        internal readonly Subject<Order[]> OrdersSubject = new Subject<Order[]>();
        internal readonly Subject<Order> OrderCreatedSubject = new Subject<Order>();
        internal readonly Subject<Order> OrderUpdatedSubject = new Subject<Order>();
        internal readonly Subject<Order> OrderCanceledSubject = new Subject<Order>();


        /// <summary>
        /// Info about every occurred error
        /// </summary>
        public IObservable<ErrorResponse> ErrorStream => ErrorSubject.AsObservable();

        /// <summary>
        /// Initial info stream, publishes always on a new connection
        /// </summary>
        public IObservable<InfoResponse> InfoStream => InfoSubject.AsObservable();

        /// <summary>
        /// Pong stream to match every ping request
        /// </summary>
        public IObservable<PongResponse> PongStream => PongSubject.AsObservable();

        /// <summary>
        /// Info about processed authentication
        /// </summary>
        public IObservable<AuthenticationResponse> AuthenticationStream => AuthenticationSubject.AsObservable();

        /// <summary>
        /// Info about processed configuration
        /// </summary>
        public IObservable<ConfigurationResponse> ConfigurationStream => ConfigurationSubject.AsObservable();

        /// <summary>
        /// Info about subscribed channel, you need to store channel id in order to future unsubscription
        /// </summary>
        public IObservable<SubscribedResponse> SubscriptionStream => SubscriptionSubject.AsObservable();

        /// <summary>
        /// Info about unsubscription
        /// </summary>
        public IObservable<UnsubscribedResponse> UnsubscriptionStream => UnsubscriptionSubject.AsObservable();

        /// <summary>
        /// Public ticker stream for subscribed pair.
        /// The ticker is a high level overview of the state of the market. It shows you the current best bid and ask, as well as the last trade price.
        /// It also includes information such as daily volume and how much the price has moved over the last day.
        /// </summary>
        public IObservable<Ticker> TickerStream => TickerSubject.AsObservable();

        /// <summary>
        /// Public trades stream for subscribed pair.
        /// This channel sends a trade message whenever a trade occurs at Bitfinex. It includes all the pertinent details of the trade, such as price, size and time.
        /// </summary>
        public IObservable<Trade> TradesStream => TradesSubject.AsObservable();

        /// <summary>
        /// Public funding stream for subscribed pair
        /// </summary>
        public IObservable<Funding> FundingStream => FundingsSubject.AsObservable();

        /// <summary>
        /// Public candles stream for subscribed pair.
        /// Provides a way to access charting candle info
        /// </summary>
        public IObservable<Candles> CandlesStream => CandlesSubject.AsObservable();

        /// <summary>
        /// Public order book stream, contains also values from initial snapshot.
        /// The Order Books channel allow you to keep track of the state of the Bitfinex order book.
        /// It is provided on a price aggregated basis, with customizable precision.
        /// After receiving the response, you will receive a snapshot of the book,
        /// followed by updates upon any changes to the book.
        /// </summary>
        public IObservable<Book> BookStream => BookSubject.AsObservable();

        /// <summary>
        /// Public initial snapshot of the order book 
        /// </summary>
        public IObservable<Book[]> BookSnapshotStream => BookSnapshotSubject.AsObservable();

        /// <summary>
        /// Private initial info about all wallets/balances (streamed only on authentication)
        /// </summary>
        public IObservable<Wallet[]> WalletsStream => WalletsSubject.AsObservable();

        /// <summary>
        /// Private stream for every wallet balance update (initial wallets info is also streamed, same as 'WalletsStream')
        /// </summary>
        public IObservable<Wallet> WalletStream => WalletSubject.AsObservable();


        /// <summary>
        /// Private initial info about all opened orders (streamed only on authentication)
        /// </summary>
        public IObservable<Order[]> OrdersStream => OrdersSubject.AsObservable();

        /// <summary>
        /// Private info about created/placed order
        /// </summary>
        public IObservable<Order> OrderCreatedStream => OrderCreatedSubject.AsObservable();

        /// <summary>
        /// Private info about updated order
        /// </summary>
        public IObservable<Order> OrderUpdatedStream => OrderUpdatedSubject.AsObservable();

        /// <summary>
        /// Private info about canceled or executed order
        /// </summary>
        public IObservable<Order> OrderCanceledStream => OrderCanceledSubject.AsObservable();


        internal BitfinexClientStreams()
        {
        }
    }
}
