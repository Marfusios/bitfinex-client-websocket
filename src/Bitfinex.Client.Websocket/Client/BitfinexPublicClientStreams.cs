using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Responses;
using Bitfinex.Client.Websocket.Responses.Books;
using Bitfinex.Client.Websocket.Responses.Candles;
using Bitfinex.Client.Websocket.Responses.FundingTrades;
using Bitfinex.Client.Websocket.Responses.Status;
using Bitfinex.Client.Websocket.Responses.Tickers;
using Bitfinex.Client.Websocket.Responses.Trades;

namespace Bitfinex.Client.Websocket.Client;

/// <summary>
/// Public websocket streams.
/// </summary>
public class BitfinexPublicClientStreams : BitfinexClientStreams
{
    /// <summary>
    /// Public ticker stream for subscribed pair.
    /// The ticker is a high level overview of the state of the market. It shows you the current best bid and ask, as well as the last trade price.
    /// It also includes information such as daily volume and how much the price has moved over the last day.
    /// </summary>
    public readonly Subject<Ticker> TickerStream = new();

    /// <summary>
    /// Info about subscribed channel, you need to store channel id in order to future unsubscription
    /// </summary>
    public readonly Subject<SubscribedResponse> SubscriptionStream = new();

    /// <summary>
    /// Info about unsubscription
    /// </summary>
    public readonly Subject<UnsubscribedResponse> UnsubscriptionStream = new();

    /// <summary>
    /// Public trades stream for subscribed pair.
    /// This channel sends a trade message whenever a trade occurs at Bitfinex. It includes all the pertinent details of the trade, such as price, size and time.
    /// </summary>
    public readonly Subject<Trade> TradesStream = new();

    /// <summary>
    /// Public trades snapshot stream for subscribed pair. It streams only initial snapshot after a reconnection.
    /// </summary>
    public readonly Subject<Trade[]> TradesSnapshotStream = new();

    /// <summary>
    /// Public funding stream for subscribed pair
    /// </summary>
    public readonly Subject<FundingTrade> FundingTradesStream = new();

    /// <summary>
    /// Public candles stream for subscribed pair.
    /// Provides a way to access charting candle info
    /// </summary>
    public readonly Subject<Candles> CandlesStream = new();

    /// <summary>
    /// Public initial snapshot of the order book 
    /// </summary>
    public readonly Subject<Book[]> BookSnapshotStream = new();

    /// <summary>
    /// Public order book stream, contains also values from initial snapshot.
    /// The Order Books channel allow you to keep track of the state of the Bitfinex order book.
    /// It is provided on a price aggregated basis, with customizable precision.
    /// After receiving the response, you will receive a snapshot of the book,
    /// followed by updates upon any changes to the book.
    /// </summary>
    public readonly Subject<Book> BookStream = new();

    /// <summary>
    /// Public initial snapshot of the order book (raw - every single order)
    /// </summary>
    public readonly Subject<RawBook[]> RawBookSnapshotStream = new();

    /// <summary>
    /// Public order book stream (raw - every single order), contains also values from initial snapshot.
    /// The Order Books channel allow you to keep track of the state of the Bitfinex order book.
    /// It provides the most granular books, every single order with order id.
    /// After receiving the response, you will receive a snapshot of the book orders,
    /// followed by updates upon any changes to the book orders.
    /// </summary>
    public readonly Subject<RawBook> RawBookStream = new();

    /// <summary>
    /// Checksum stream for every book iteration. Checks the top 25 entries for each side of book. Checksum is a signed int.
    /// Must be enabled by configuration (see `ConfigurationRequest`)
    /// </summary>
    public readonly Subject<ChecksumResponse> BookChecksumStream = new();

    /// <summary>
    /// Public info about a derivative symbol
    /// </summary>
    public readonly Subject<DerivativePairStatus> DerivativePairStream = new();

    /// <summary>
    /// Public liquidation feed
    /// </summary>
    public readonly Subject<LiquidationFeedStatus> LiquidationFeedStream = new();
}