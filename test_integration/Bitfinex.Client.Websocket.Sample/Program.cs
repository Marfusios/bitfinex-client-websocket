using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using Bitfinex.Client.Websocket.Client;
using Bitfinex.Client.Websocket.Requests;
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
                using (var client = new BitfinexWebsocketClient(communicator))
                {

                    client.Streams.PongStream.Subscribe(pong => Log.Information($"Pong received! Id: {pong.Cid}"));
                    client.Streams.TickerStream.Subscribe(ticker => Log.Information($"{ticker.Pair} - last price: {ticker.LastPrice}"));

                    client.Streams.CandlesStream.Subscribe(candles =>
                    {
                        candles.CandleList.OrderBy(x => x.TimeStamp).ToList().ForEach(x =>
                        {
                            Log.Information(
                                $"Candle(Pair : {candles.Pair} TimeFrame : {candles.TimeFrame.GetStringValue()}) --> {x.TimeStamp} High : {x.High} Low : {x.Low} Open : {x.Open} Close : {x.Close}");
                        });
                    });

                    client.Streams.AuthenticationStream.Subscribe(auth => Log.Information($"Authenticated: {auth.IsAuthenticated}"));
                    client.Streams.WalletsStream
                        .Subscribe(wallets => wallets.ToList().ForEach(wallet =>
                            Log.Information($"Wallet {wallet.Currency} balance: {wallet.Balance}")));

                    communicator.Start().Wait();

                    client.Send(new PingRequest() { Cid = 123456 });
                    client.Send(new TickerSubscribeRequest("BTC/USD"));
                    client.Send(new TickerSubscribeRequest("ETH/USD"));
                    client.Send(new CandlesSubscribeRequest("BTC/USD", BitfinexTimeFrame.OneMinute));
                    client.Send(new CandlesSubscribeRequest("ETH/USD", BitfinexTimeFrame.OneMinute));

                    if (!string.IsNullOrWhiteSpace(API_SECRET))
                    {
                        client.Authenticate(API_KEY, API_SECRET);

                        // Place BUY order
                        // client.Send(new NewOrderRequest(33, 1, "ETH/USD", OrderType.ExchangeLimit, 0.2, 100));

                        // Place SELL order
                        // client.Send(new NewOrderRequest(33, 2, "ETH/USD", OrderType.ExchangeLimit, -0.2, 2000));

                        // Cancel order
                        // client.Send(new CancelOrderRequest(1));
                    }

                    ExitEvent.WaitOne();
                }
            }

            Log.Debug("====================================");
            Log.Debug("              STOPPING              ");
            Log.Debug("====================================");
            Log.CloseAndFlush();
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
