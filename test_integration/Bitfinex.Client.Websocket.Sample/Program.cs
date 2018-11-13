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
using Bitfinex.Client.Websocket.Responses.Fundings;
using Bitfinex.Client.Websocket.Responses.Trades;
using Bitfinex.Client.Websocket.Utils;
using Bitfinex.Client.Websocket.Websockets;
using Serilog;
using Serilog.Events;

namespace Bitfinex.Client.Websocket.Sample
{
    class Program
    {
        private static readonly ManualResetEvent ExitEvent = new ManualResetEvent(false);

        private static readonly string API_KEY = "your_api_key";
        private static readonly string API_SECRET = "";

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


            var url = BitfinexValues.ApiWebsocketUrl;
            using (var communicator = new BitfinexWebsocketCommunicator(url))
            {
                communicator.ReconnectTimeoutMs = (int)TimeSpan.FromSeconds(30).TotalMilliseconds;
                communicator.ReconnectionHappened.Subscribe(type =>
                    Log.Information($"Reconnection happened, type: {type}"));

                using (var client = new BitfinexWebsocketClient(communicator))
                {
                    client.Streams.InfoStream.Subscribe(info =>
                    {
                        Log.Information($"Info received version: {info.Version}, reconnection happened, resubscribing to streams");
                        SendSubscriptionRequests(client).Wait();
                    });

                    SubscribeToStreams(client);

                    communicator.Start();

                    ExitEvent.WaitOne();
                }
            }

            Log.Debug("====================================");
            Log.Debug("              STOPPING              ");
            Log.Debug("====================================");
            Log.CloseAndFlush();
        }

        private static async Task SendSubscriptionRequests(BitfinexWebsocketClient client)
        {
            await client.Send(new PingRequest() {Cid = 123456});

            //await client.Send(new TickerSubscribeRequest("BTC/USD"));
            //await client.Send(new TickerSubscribeRequest("ETH/USD"));

            await client.Send(new TradesSubscribeRequest("BTC/USD"));
            //await client.Send(new FundingsSuscribeRequest("BTC"));
            //await client.Send(new FundingsSuscribeRequest("USD"));

            //await client.Send(new CandlesSubscribeRequest("BTC/USD", BitfinexTimeFrame.OneMinute));
            //await client.Send(new CandlesSubscribeRequest("ETH/USD", BitfinexTimeFrame.OneMinute));

            await client.Send(new BookSubscribeRequest("BTC/USD", BitfinexPrecision.P0, BitfinexFrequency.Realtime));
            //await client.Send(new BookSubscribeRequest("BTC/USD", BitfinexPrecision.P3, BitfinexFrequency.Realtime));

            if (!string.IsNullOrWhiteSpace(API_SECRET))
            {
                await client.Authenticate(API_KEY, API_SECRET);

                // Place BUY order
                // await client.Send(new NewOrderRequest(33, 1, "ETH/USD", OrderType.ExchangeLimit, 0.2, 100));

                // Place SELL order
                // await client.Send(new NewOrderRequest(33, 2, "ETH/USD", OrderType.ExchangeLimit, -0.2, 2000));

                // Cancel order
                // await client.Send(new CancelOrderRequest(1));
            }
        }

        private static void SubscribeToStreams(BitfinexWebsocketClient client)
        {
            client.Streams.PongStream.Subscribe(pong => Log.Information($"Pong received! Id: {pong.Cid}"));
            client.Streams.TickerStream.Subscribe(ticker =>
                Log.Information($"{ticker.Pair} - last price: {ticker.LastPrice}, bid: {ticker.Bid}, ask: {ticker.Ask}"));
            client.Streams.TradesStream.Where(x => x.Type == TradeType.Executed).Subscribe(x =>
                Log.Information($"Trade {x.Pair} executed. Time: {x.Mts:mm:ss.fff}, Amount: {x.Amount}, Price: {x.Price}"));
            client.Streams.FundingStream.Where(x => x.Type == FundingType.Executed).Subscribe(x =>
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
                    $"Book | channel: {book.ChanId} pair: {book.Pair}, price: {book.Price}, amount {book.Amount}, count: {book.Count}"));

            client.Streams.CandlesStream.Subscribe(candles =>
            {
                candles.CandleList.OrderBy(x => x.Mts).ToList().ForEach(x =>
                {
                    Log.Information(
                        $"Candle(Pair : {candles.Pair} TimeFrame : {candles.TimeFrame.GetStringValue()}) --> {x.Mts} High : {x.High} Low : {x.Low} Open : {x.Open} Close : {x.Close}");
                });
            });

            client.Streams.AuthenticationStream.Subscribe(auth => Log.Information($"Authenticated: {auth.IsAuthenticated}"));
            client.Streams.WalletStream
                .Subscribe(wallet =>
                    Log.Information($"Wallet {wallet.Currency} balance: {wallet.Balance} type: {wallet.Type}"));
        }

        private static void InitLogging()
        {
            var executingDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var logPath = Path.Combine(executingDir, "logs", "verbose.log");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                .WriteTo.ColoredConsole(LogEventLevel.Information)
                .CreateLogger();
        }

        private static void CurrentDomainOnProcessExit(object sender, EventArgs eventArgs)
        {
            Log.Warning("Exiting process");
            ExitEvent.Set();
        }

        private static void DefaultOnUnloading(AssemblyLoadContext assemblyLoadContext)
        {
            Log.Warning("Unloading process");
            ExitEvent.Set();
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Log.Warning("Canceling process");
            e.Cancel = true;
            ExitEvent.Set();
        }
    }
}
