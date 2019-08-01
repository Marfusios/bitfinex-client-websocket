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
                communicator.Name = "Bitfinex-1";
                communicator.ReconnectTimeoutMs = (int)TimeSpan.FromSeconds(300).TotalMilliseconds;
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
            //await client.Send(new ConfigurationRequest(ConfigurationFlag.Timestamp | ConfigurationFlag.Sequencing));
            await client.Send(new PingRequest() {Cid = 123456});

            //await client.Send(new TickerSubscribeRequest("BTC/USD"));
            //await client.Send(new TickerSubscribeRequest("ETH/USD"));

            await client.Send(new TradesSubscribeRequest("BTC/USD"));
            //await client.Send(new TradesSubscribeRequest("NEC/ETH")); // Nectar coin from ETHFINEX
            //await client.Send(new FundingsSubscribeRequest("BTC"));
            //await client.Send(new FundingsSubscribeRequest("USD"));

            //await client.Send(new CandlesSubscribeRequest("BTC/USD", BitfinexTimeFrame.OneMinute));
            //await client.Send(new CandlesSubscribeRequest("ETH/USD", BitfinexTimeFrame.OneMinute));

            //await client.Send(new BookSubscribeRequest("BTC/USD", BitfinexPrecision.P0, BitfinexFrequency.Realtime));
            //await client.Send(new BookSubscribeRequest("BTC/USD", BitfinexPrecision.P3, BitfinexFrequency.Realtime));
            //await client.Send(new BookSubscribeRequest("ETH/USD", BitfinexPrecision.P0, BitfinexFrequency.Realtime));

            //await client.Send(new StatusSubscribeRequest("liq:global"));
            //await client.Send(new StatusSubscribeRequest("deriv:tBTCF0:USTF0"));

            if (!string.IsNullOrWhiteSpace(API_SECRET))
            {
                await client.Authenticate(API_KEY, API_SECRET);

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

        private static void SubscribeToStreams(BitfinexWebsocketClient client)
        {
            // public streams:

            client.Streams.ConfigurationStream.Subscribe(x =>
                Log.Information($"Configuration happened {x.Status}, flags: {x.Flags}, server timestamp enabled: {client.Configuration.IsTimestampEnabled}"));

            client.Streams.PongStream.Subscribe(pong => Log.Information($"Pong received! Id: {pong.Cid}"));
            
            client.Streams.TickerStream.Subscribe(ticker =>
                Log.Information($"{ticker.ServerSequence} {ticker.Pair} - last price: {ticker.LastPrice}, bid: {ticker.Bid}, ask: {ticker.Ask}, {ShowServerTimestamp(client, ticker)}"));
            
            client.Streams.TradesSnapshotStream.Subscribe(trades =>
            {
                foreach (var x in trades)
                {
                    Log.Information(
                        $"{x.ServerSequence} Trade {x.Pair} from snapshot. Time: {x.Mts:mm:ss.fff}, Amount: {x.Amount}, Price: {x.Price}, {ShowServerTimestamp(client, x)}");
                }
            });

            client.Streams.TradesStream.Where(x => x.Type == TradeType.Executed).Subscribe(x =>
                Log.Information($"{x.ServerSequence} Trade {x.Pair} executed. Time: {x.Mts:mm:ss.fff}, Amount: {x.Amount}, Price: {x.Price}, {ShowServerTimestamp(client, x)}"));
            
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
                    $"{book.ServerSequence} Book | channel: {book.ChanId} pair: {book.Pair}, price: {book.Price}, amount {book.Amount}, count: {book.Count}, {ShowServerTimestamp(client, book)}"));

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




            // Private streams:

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

            client.Streams.PrivateTradeStream.Subscribe(trade => 
                Log.Information($"Private trade {trade.Pair} executed. Time: {trade.MtsCreate:mm:ss.fff}, Amount: {trade.ExecAmount}, Price: {trade.ExecPrice}, " +
                                $"Fee: {trade.Fee} {trade.FeeCurrency}, type: {trade.OrderType}, " +
                                $"{ShowServerSequence(client, trade)}, {ShowServerTimestamp(client, trade)}"));


            client.Streams.PositionsStream.Subscribe(positions =>
            {
                foreach (var info in positions)
                {
                    Log.Information($"Position snapshot: {info.Pair} - {info.Status} - {info.Amount} @ {info.BasePrice} " +
                                    $"| PL: {info.ProfitLoss} {info.ProfitLossPercentage}% " +
                                    $"{ShowServerSequence(client, info)}, {ShowServerTimestamp(client, info)}");
                }
            });

            client.Streams.PositionCreatedStream.Subscribe(info =>
                Log.Information($"Position created: {info.Pair} - {info.Status} - {info.Amount} @ {info.BasePrice} " +
                                $"| PL: {info.ProfitLoss} {info.ProfitLossPercentage}% " +
                                $"{ShowServerTimestamp(client, info)}"));

            client.Streams.PositionUpdatedStream.Subscribe(info =>
                Log.Information($"Position updated: {info.Pair} - {info.Status} - {info.Amount} @ {info.BasePrice} " +
                                $"| PL: {info.ProfitLoss} {info.ProfitLossPercentage}% " +
                                $"{ShowServerTimestamp(client, info)}"));

            client.Streams.PositionCanceledStream.Subscribe(info =>
                Log.Information($"Position canceled: {info.Pair} - {info.Status} - {info.Amount} @ {info.BasePrice} " +
                                $"| PL: {info.ProfitLoss} {info.ProfitLossPercentage}% " +
                                $"{ShowServerTimestamp(client, info)}"));

            client.Streams.NotificationStream.Subscribe(notification =>
                Log.Information(
                    $"Notification: {notification.Text} code: {notification.Code}, status: {notification.Status}, type : {notification.Type}"));
            
            client.Streams.BalanceInfoStream.Subscribe(info =>
                Log.Information($"Balance, total: {info.TotalAum}, net: {info.NetAum}"));

            client.Streams.MarginInfoStream.Subscribe(info =>
                Log.Information(
                    $"Margin, balance: {info.MarginBalance}, required: {info.MarginRequired}, net: {info.MarginNet}, p/l: {info.UserPl}, swaps: {info.UserSwaps}"));


            client.Streams.DerivativePairStream.Subscribe(info =>
            {
                Log.Information(
                    $"Derivative status, symbol: {info.Symbol}, derivPrice: {info.DerivPrice}, spot price: {info.SpotPrice}, insurance fund balance: {info.InsuranceFundBalance}, funding: {info.FundingAccrued}, funding step: {info.FundingStep}");
            });

            client.Streams.LiquidationFeedStream.Subscribe(info =>
            {
                Log.Information(
                    $"Liquidation, symbol: {info.Symbol}, position id: {info.PosId}, amount: {info.Amount}, base price: {info.BasePrice}, is match: {info.IsMatch}, market sold: {info.IsMarketSold}");
            });
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

        private static string ShowServerTimestamp(BitfinexWebsocketClient client, ResponseBase response)
        {
            if(!client.Configuration.IsTimestampEnabled)
                return string.Empty;
            return $"server timestamp: {response.ServerTimestamp:mm:ss.fff}";
        }

        private static string ShowServerSequence(BitfinexWebsocketClient client, ResponseBase response)
        {
            if(!client.Configuration.IsSequencingEnabled)
                return string.Empty;
            return $"sequence: {response.ServerSequence} / {response.ServerPrivateSequence}";
        }

        private static void InitLogging()
        {
            var executingDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var logPath = Path.Combine(executingDir, "logs", "verbose.log");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                .WriteTo.ColoredConsole(LogEventLevel.Information, outputTemplate: 
                    "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
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
