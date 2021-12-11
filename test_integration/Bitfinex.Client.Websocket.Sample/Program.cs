using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Client.Websocket.Client;
using Bitfinex.Client.Websocket.Requests;
using Bitfinex.Client.Websocket.Requests.Subscriptions;
using Bitfinex.Client.Websocket.Responses;
using Bitfinex.Client.Websocket.Responses.Configurations;
using Bitfinex.Client.Websocket.Utils;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using Serilog.Events;
using Websocket.Client;

namespace Bitfinex.Client.Websocket.Sample;

class Program
{
    static readonly ManualResetEvent ExitEvent = new(false);

    static readonly string API_KEY = "your_api_key";
    static readonly string API_SECRET = "";

    static void Main(string[] args)
    {
        InitLogging();

        AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
        AssemblyLoadContext.Default.Unloading += DefaultOnUnloading;
        Console.CancelKeyPress += ConsoleOnCancelKeyPress;

        Console.WriteLine("|=======================|");
        Console.WriteLine("|    BITFINEX CLIENT    |");
        Console.WriteLine("|=======================|");
        Console.WriteLine();

        Log.Debug("====================================");
        Log.Debug("              STARTING              ");
        Log.Debug("====================================");


        using var publicApiClient = new WebsocketClient(BitfinexValues.BitfinexPublicWebsocketUrl);
        publicApiClient.Name = "Bitfinex-1";
        publicApiClient.ReconnectTimeout = TimeSpan.FromSeconds(30);
        publicApiClient.ReconnectionHappened.Subscribe(info =>
            Log.Information($"Reconnection happened, type: {info.Type}"));

        using var publicClient = new BitfinexPublicWebsocketClient(NullLogger.Instance, publicApiClient);
        publicClient.Streams.InfoStream.Subscribe(info =>
        {
            Log.Information($"Info received version: {info.Version}, reconnection happened, resubscribing to streams");
            SendSubscriptionRequests(publicClient).Wait();
        });

        SubscribeToPublicStreams(publicClient);

        using var authenticatedApiClient = new WebsocketClient(BitfinexValues.BitfinexWebsocketUrl);
        publicApiClient.Name = "Bitfinex-2";
        publicApiClient.ReconnectTimeout = TimeSpan.FromSeconds(30);
        publicApiClient.ReconnectionHappened.Subscribe(info =>
            Log.Information($"Reconnection happened, type: {info.Type}"));

        using var authenticatedClient = new BitfinexAuthenticatedWebsocketClient(NullLogger.Instance, authenticatedApiClient);
        publicClient.Streams.InfoStream.Subscribe(info =>
        {
            Log.Information($"Info received version: {info.Version}, reconnection happened");
        });

        SubscribeToAuthenticatedStreams(authenticatedClient);

        publicApiClient.Start();
        authenticatedApiClient.Start();

        ExitEvent.WaitOne();

        Log.Debug("====================================");
        Log.Debug("              STOPPING              ");
        Log.Debug("====================================");
        Log.CloseAndFlush();
    }

    static async Task SendSubscriptionRequests(BitfinexPublicWebsocketClient client)
    {
        //client.Send(new ConfigurationRequest(ConfigurationFlag.Timestamp | ConfigurationFlag.Sequencing));
        client.Send(new PingRequest { Cid = 123456 });

        //client.Send(new TickerSubscribeRequest("BTC/USD"));
        //client.Send(new TickerSubscribeRequest("ETH/USD"));

        //client.Send(new TradesSubscribeRequest("BTC/USD"));
        //client.Send(new TradesSubscribeRequest("NEC/ETH")); // Nectar coin from ETHFINEX
        //client.Send(new FundingsSubscribeRequest("BTC"));
        //client.Send(new FundingsSubscribeRequest("USD"));

        //client.Send(new CandlesSubscribeRequest("BTC/USD", BitfinexTimeFrame.OneMinute));
        //client.Send(new CandlesSubscribeRequest("ETH/USD", BitfinexTimeFrame.OneMinute));

        //client.Send(new BookSubscribeRequest("BTC/USD", BitfinexPrecision.P0, BitfinexFrequency.Realtime));
        //client.Send(new BookSubscribeRequest("BTC/USD", BitfinexPrecision.P3, BitfinexFrequency.Realtime));
        //client.Send(new BookSubscribeRequest("ETH/USD", BitfinexPrecision.P0, BitfinexFrequency.Realtime));

        //client.Send(new BookSubscribeRequest("fUSD", BitfinexPrecision.P0, BitfinexFrequency.Realtime));

        client.Send(new RawBookSubscribeRequest("BTCUSD", "100"));
        //client.Send(new RawBookSubscribeRequest("fUSD", "25"));
        //client.Send(new RawBookSubscribeRequest("fBTC", "25"));

        //client.Send(new StatusSubscribeRequest("liq:global"));
        //client.Send(new StatusSubscribeRequest("deriv:tBTCF0:USTF0"));

        if (!string.IsNullOrWhiteSpace(API_SECRET))
        {
            client.Send(new AuthenticationRequest(API_KEY, API_SECRET));

#pragma warning disable 4014
            Task.Run(async () =>
#pragma warning restore 4014
            {
                Task.Delay(2000).Wait();

                // Place BUY order
                //await client.Send(new NewOrderRequest(1, 100, "ETH/USD", OrderType.Limit, 0.2, 103) {Flags = OrderFlag.PostOnly});
                //await client.Send(new NewOrderRequest(2, 101, "ETH/USD", OrderType.Limit, 0.2, 102) {Flags = OrderFlag.PostOnly});
                //await client.Send(new NewOrderRequest(33, 102, "ETH/USD", OrderType.Limit, 0.2, 101) {Flags = OrderFlag.PostOnly});

                // Place SELL order
                //await client.Send(new NewOrderRequest(1, 200, "ETH/USD", OrderType.Limit, -0.2, 108) {Flags = OrderFlag.PostOnly});
                //await client.Send(new NewOrderRequest(2, 201, "ETH/USD", OrderType.Limit, -0.2, 109) {Flags = OrderFlag.PostOnly});
                //await client.Send(new NewOrderRequest(33, 202, "ETH/USD", OrderType.Limit, -0.2, 110) {Flags = OrderFlag.PostOnly});

                Task.Delay(7000).Wait();

                // Cancel order separately
                //await client.Send(new CancelOrderRequest(new CidPair(100, DateTime.UtcNow)));
                //await client.Send(new CancelOrderRequest(new CidPair(200, DateTime.UtcNow)));

                Task.Delay(7000).Wait();

                // Cancel order multi
                //await client.Send(new CancelMultiOrderRequest(new[]
                //{
                //    new CidPair(101, DateTime.UtcNow),
                //    new CidPair(201, DateTime.UtcNow)
                //}));

                Task.Delay(2000).Wait();

                //await client.Send(CancelMultiOrderRequest.CancelGroup(33));
                //await client.Send(CancelMultiOrderRequest.CancelEverything());

                // request calculations
                // await client.Send(new CalcRequest(new[]
                // {
                //     "margin_base",
                //     "balance",
                // }));
            });
        }
    }

    static void SubscribeToPublicStreams(BitfinexPublicWebsocketClient client)
    {
        // public streams:

        client.Streams.ConfigurationStream.Subscribe(x =>
            Log.Information($"Configuration happened {x.Status}, flags: {x.Flags}, server timestamp enabled: {client.Configuration.IsTimestampEnabled}"));

        client.Streams.PongStream.Subscribe(pong => Log.Information($"Pong received! Id: {pong.Cid}"));

        client.Streams.TickerStream.Subscribe(ticker =>
            Log.Information($"{ticker.ServerSequence} {ticker.Pair} - last price: {ticker.LastPrice}, bid: {ticker.Bid}, ask: {ticker.Ask}, {ShowServerTimestamp(client.Configuration, ticker)}"));

        client.Streams.TradesSnapshotStream.Subscribe(trades =>
        {
            foreach (var x in trades)
            {
                Log.Information(
                    $"{x.ServerSequence} Trade {x.Pair} from snapshot. Time: {x.Mts:mm:ss.fff}, Amount: {x.Amount}, Price: {x.Price}, {ShowServerTimestamp(client.Configuration, x)}");
            }
        });

        client.Streams.TradesStream.Where(x => x.Type == TradeType.Executed).Subscribe(x =>
            Log.Information($"{x.ServerSequence} Trade {x.Pair} executed. Time: {x.Mts:mm:ss.fff}, Amount: {x.Amount}, Price: {x.Price}, {ShowServerTimestamp(client.Configuration, x)}"));

        client.Streams.FundingTradesStream.Where(x => x.Type == TradeType.Executed).Subscribe(x =>
            Log.Information($"Funding,  Symbol {x.Symbol} executed. Time: {x.Mts:mm:ss.fff}, Amount: {x.Amount}, Rate: {x.Rate}, Period: {x.Period}"));

        client.Streams.CandlesStream.Subscribe(candles =>
        {
            candles.CandleList.OrderBy(x => x.Mts).ToList().ForEach(x =>
            {
                Log.Information(
                    $"Candle(Pair : {candles.Pair} TimeFrame : {candles.TimeFrame.GetStringValue()}) --> {x.Mts} High : {x.High} Low : {x.Low} Open : {x.Open} Close : {x.Close}");
            });
        });

        client.Streams.BookStream.Subscribe(book =>
            Log.Information(
                book.Period <= 0 ?
                    $"{book.ServerSequence} Book | channel: {book.ChanId} pair: {book.Pair}, price: {book.Price}, amount {book.Amount}, count: {book.Count}, {ShowServerTimestamp(client.Configuration, book)}" :
                    $"{book.ServerSequence} Book | channel: {book.ChanId} sym: {book.Symbol}, rate: {book.Rate * 100}% (p.a. {(book.Rate * 100 * 365):F}%), period: {book.Period} amount {book.Amount}, count: {book.Count}, {ShowServerTimestamp(client.Configuration, book)}"));

        client.Streams.RawBookStream.Subscribe(book =>
        {
            Log.Information(
                book.OrderId > 0
                    ? $"{book.ServerSequence} RawBook | channel: {book.ChanId} pair: {book.Pair}, order: {book.OrderId}, price: {book.Price}, amount {book.Amount} {ShowServerTimestamp(client.Configuration, book)}"
                    : $"{book.ServerSequence} RawBook | channel: {book.ChanId} sym: {book.Symbol}, offer: {book.OfferId}, period: {book.Period} days, rate {book.Rate * 100}% (p.a. {(book.Rate * 100 * 365):F}%) {ShowServerTimestamp(client.Configuration, book)}");
        });


        client.Streams.CandlesStream.Subscribe(candles =>
        {
            candles.CandleList.OrderBy(x => x.Mts).ToList().ForEach(x =>
            {
                Log.Information(
                    $"Candle(Pair : {candles.Pair} TimeFrame : {candles.TimeFrame.GetStringValue()}) --> {x.Mts} High : {x.High} Low : {x.Low} Open : {x.Open} Close : {x.Close}");
            });
        });

        client.Streams.BookChecksumStream.Subscribe(x =>
            Log.Information($"{x.ServerSequence} [CHECKSUM] {x.Pair}-{x.ChanId}  {x.Checksum}"));

        // Unsubscription example: 

        //client.Streams.SubscriptionStream.ObserveOn(TaskPoolScheduler.Default).Subscribe(info =>
        //{
        //    if(!info.Channel.Contains("book"))
        //        return;
        //    Task.Delay(5000).Wait();
        //    var channelId = info.ChanId;
        //    client.Send(new UnsubscribeRequest() {ChanId = channelId}).Wait();
        //});
    }

    static void SubscribeToAuthenticatedStreams(BitfinexAuthenticatedWebsocketClient client)
    {
        client.Streams.AuthenticationStream.Subscribe(auth => Log.Information($"Authenticated: {auth.IsAuthenticated}"));
        client.Streams.WalletStream
            .Subscribe(wallet =>
                Log.Information($"Wallet {wallet.Currency} balance: {wallet.Balance} type: {wallet.Type}"));

        client.Streams.OrdersStream.Subscribe(orders =>
        {
            foreach (var info in orders)
            {
                Log.Information($"Order #{info.Cid} group: {info.Gid} snapshot: {info.Pair} - {info.Type} - {info.Amount} @ {info.Price} | {info.OrderStatus}");
            }
        });

        client.Streams.OrderCreatedStream.Subscribe(async info =>
        {
            Log.Information(
                $"Order #{info.Cid} group: {info.Gid} created: {info.Pair} - {info.Type} - {info.Amount} @ {info.Price} | {info.OrderStatus}");

            // Update order
            //await Task.Delay(5000);
            //await client.Send(new UpdateOrderRequest(info.Id)
            //{
            //    Price = info.Price - 1,
            //    Amount = info.Amount + 0.1 * Math.Sign(info.Amount ?? 0),
            //    Flags = OrderFlag.PostOnly
            //});
        });


        client.Streams.OrderUpdatedStream.Subscribe(info =>
            Log.Information($"Order #{info.Cid} group: {info.Gid} updated: {info.Pair} - {info.Type} - {info.Amount} @ {info.Price} | {info.OrderStatus}"));

        client.Streams.OrderCanceledStream.Subscribe(info =>
            Log.Information($"Order #{info.Cid} group: {info.Gid} {info.OrderStatus}: {info.Pair} - {info.Type} - {info.Amount} @ {info.Price}"));

        client.Streams.PrivateTradesStream.Subscribe(trade =>
            Log.Information($"Private trade {trade.Pair} executed. Time: {trade.MtsCreate:mm:ss.fff}, Amount: {trade.ExecAmount}, Price: {trade.ExecPrice}, " +
                            $"Fee: {trade.Fee} {trade.FeeCurrency}, type: {trade.OrderType}, " +
                            $"{ShowServerSequence(client.Configuration, trade)}, {ShowServerTimestamp(client.Configuration, trade)}"));


        client.Streams.PositionsStream.Subscribe(positions =>
        {
            foreach (var info in positions)
            {
                Log.Information($"Position snapshot: {info.Pair} - {info.Status} - {info.Amount} @ {info.BasePrice} " +
                                $"| PL: {info.ProfitLoss} {info.ProfitLossPercentage}% " +
                                $"{ShowServerSequence(client.Configuration, info)}, {ShowServerTimestamp(client.Configuration, info)}");
            }
        });

        client.Streams.PositionCreatedStream.Subscribe(info =>
            Log.Information($"Position created: {info.Pair} - {info.Status} - {info.Amount} @ {info.BasePrice} " +
                            $"| PL: {info.ProfitLoss} {info.ProfitLossPercentage}% " +
                            $"{ShowServerTimestamp(client.Configuration, info)}"));

        client.Streams.PositionUpdatedStream.Subscribe(info =>
            Log.Information($"Position updated: {info.Pair} - {info.Status} - {info.Amount} @ {info.BasePrice} " +
                            $"| PL: {info.ProfitLoss} {info.ProfitLossPercentage}% " +
                            $"{ShowServerTimestamp(client.Configuration, info)}"));

        client.Streams.PositionCanceledStream.Subscribe(info =>
            Log.Information($"Position canceled: {info.Pair} - {info.Status} - {info.Amount} @ {info.BasePrice} " +
                            $"| PL: {info.ProfitLoss} {info.ProfitLossPercentage}% " +
                            $"{ShowServerTimestamp(client.Configuration, info)}"));

        client.Streams.NotificationStream.Subscribe(notification =>
            Log.Information(
                $"Notification: {notification.Text} code: {notification.Code}, status: {notification.Status}, type : {notification.Type}"));

        client.Streams.BalanceInfoStream.Subscribe(info =>
            Log.Information($"Balance, total: {info.TotalAum}, net: {info.NetAum}"));

        client.Streams.MarginInfoStream.Subscribe(info =>
            Log.Information(
                $"Margin, balance: {info.MarginBalance}, required: {info.MarginRequired}, net: {info.MarginNet}, p/l: {info.UserPl}, swaps: {info.UserSwaps}"));
    }

    static string ShowServerTimestamp(ConfigurationState configuration, ResponseBase response)
    {
        if (!configuration.IsTimestampEnabled)
            return string.Empty;
        return $"server timestamp: {response.ServerTimestamp:mm:ss.fff}";
    }

    static string ShowServerSequence(ConfigurationState configuration, ResponseBase response)
    {
        if (!configuration.IsSequencingEnabled)
            return string.Empty;
        return $"sequence: {response.ServerSequence} / {response.ServerPrivateSequence}";
    }

    static void InitLogging()
    {
        var executingDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        var logPath = Path.Combine(executingDir, "logs", "verbose.log");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
            .WriteTo.ColoredConsole(LogEventLevel.Debug, outputTemplate:
                "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    static void CurrentDomainOnProcessExit(object sender, EventArgs eventArgs)
    {
        Log.Warning("Exiting process");
        ExitEvent.Set();
    }

    static void DefaultOnUnloading(AssemblyLoadContext assemblyLoadContext)
    {
        Log.Warning("Unloading process");
        ExitEvent.Set();
    }

    static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
        Log.Warning("Canceling process");
        e.Cancel = true;
        ExitEvent.Set();
    }
}