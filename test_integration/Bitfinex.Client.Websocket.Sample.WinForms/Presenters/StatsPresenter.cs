using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Bitfinex.Client.Websocket;
using Bitfinex.Client.Websocket.Client;
using Bitfinex.Client.Websocket.Communicator;
using Bitfinex.Client.Websocket.Requests;
using Bitfinex.Client.Websocket.Responses;
using Bitfinex.Client.Websocket.Responses.Books;
using Bitfinex.Client.Websocket.Responses.Trades;
using Bitmex.Client.Websocket.Sample.WinForms.Statistics;
using Bitmex.Client.Websocket.Sample.WinForms.Views;
using Bitfinex.Client.Websocket.Websockets;
using Serilog;
using Websocket.Client;

namespace Bitmex.Client.Websocket.Sample.WinForms.Presenters
{
    class StatsPresenter
    {
        private readonly object _gate = new object();
        private readonly IStatsView _view;

        private TradeStatsComputer _tradeStatsComputer;
        private OrderBookStatsComputer _orderBookStatsComputer;

        private IBitfinexCommunicator _communicator;
        private BitfinexWebsocketClient _client;

        private IDisposable _pingSubscription;
        private Stopwatch _pingRequest = Stopwatch.StartNew();

        private string _defaultPair = "BTCUSD";
        private string _currency = "$";

        public StatsPresenter(IStatsView view)
        {
            _view = view;

            HandleCommands();
        }

        private void HandleCommands()
        {
            _view.OnInit = OnInit;
            _view.OnStart = async () => await OnStart();
            _view.OnStop = OnStop;
        }

        private void OnInit()
        {
            Clear();
        }

        private async Task OnStart()
        {
            var pair = _view.Pair;
            if (string.IsNullOrWhiteSpace(pair))
                pair = _defaultPair;
            pair = pair.ToUpper();

            _tradeStatsComputer = new TradeStatsComputer();
            _orderBookStatsComputer = new OrderBookStatsComputer();

            var url = BitfinexValues.ApiWebsocketUrl;
            _communicator = new BitfinexWebsocketCommunicator(url);
            _client = new BitfinexWebsocketClient(_communicator);

            Subscribe(_client);

            _communicator.ReconnectionHappened.Subscribe(async type =>
            {
                _view.Status($"Reconnected (type: {type})", StatusType.Info);
                await SendSubscriptions(_client, pair);
            });

            _communicator.DisconnectionHappened.Subscribe(type =>
            {
                if (type == DisconnectionType.Error)
                {
                    _view.Status($"Disconnected by error, next try in {_communicator.ErrorReconnectTimeoutMs/1000} sec", 
                        StatusType.Error);
                    return;
                }
                _view.Status($"Disconnected (type: {type})", 
                    StatusType.Warning);
            });

            await _communicator.Start();

            StartPingCheck(_client);
        }

        private void OnStop()
        {
            _pingSubscription.Dispose();
            _client.Dispose();
            _communicator.Dispose();
            _client = null;
            _communicator = null;
            Clear();
        }

        private void Subscribe(BitfinexWebsocketClient client)
        {
            client.Streams.TradesStream
                .Where(x => x.Type == TradeType.Executed)
                .ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(HandleTrades);

            client.Streams.BookStream
                .ObserveOn(TaskPoolScheduler.Default)
                .Synchronize(_gate)
                .Subscribe(HandleOrderBook);

            client.Streams.BookSnapshotStream
                .ObserveOn(TaskPoolScheduler.Default)
                .Synchronize(_gate)
                .Subscribe(HandleOrderBook);

            client.Streams.PongStream
                .ObserveOn(TaskPoolScheduler.Default)
                .Subscribe(HandlePong);
        }

        private async Task SendSubscriptions(BitfinexWebsocketClient client, string pair)
        {
            await client.Send(new TradesSubscribeRequest(pair));
            await client.Send(new BookSubscribeRequest(pair));
        }

        private void HandleTrades(Trade trade)
        {
            Log.Information($"Received trade, price: {trade.Price}, amount: {trade.Amount}");
            _tradeStatsComputer.HandleTrade(trade);

            FormatTradesStats(_view.Trades1Min, _tradeStatsComputer.GetStatsFor(1));
            FormatTradesStats(_view.Trades5Min, _tradeStatsComputer.GetStatsFor(5));
            FormatTradesStats(_view.Trades15Min, _tradeStatsComputer.GetStatsFor(15));
            FormatTradesStats(_view.Trades1Hour, _tradeStatsComputer.GetStatsFor(60));
            FormatTradesStats(_view.Trades24Hours, _tradeStatsComputer.GetStatsFor(60 * 24));
        }

        private void FormatTradesStats(Action<string, Side> setAction, TradeStats trades)
        {
            if (trades == TradeStats.NULL)
                return;

            if (trades.BuysPerc >= trades.SellsPerc)
            {
                setAction($"{trades.BuysPerc:###}% buys{Environment.NewLine}{trades.TotalCount}", Side.Buy);
                return;
            }
            setAction($"{trades.SellsPerc:###}% sells{Environment.NewLine}{trades.TotalCount}", Side.Sell);
        }

        private void HandleOrderBook(Book book)
        {
            HandleOrderBook(new[] {book});
        }

        private void HandleOrderBook(Book[] books)
        {
            foreach (var book in books)
            {
                _orderBookStatsComputer.HandleOrderBook(book);

            }

            var stats = _orderBookStatsComputer.GetStats();
            if (stats == OrderBookStats.NULL)
                return;

            _view.Bid = stats.Bid.ToString("#.0");
            _view.Ask = stats.Ask.ToString("#.0");

            _view.BidAmount = $"{stats.BidAmountPerc:###}%{Environment.NewLine}{FormatToMilions(stats.BidAmount)}";
            _view.AskAmount = $"{stats.AskAmountPerc:###}%{Environment.NewLine}{FormatToMilions(stats.AskAmount)}";
        }

        private string FormatToMilions(double amount)
        {
            var milions = amount / 1000000;
            return $"{_currency}{milions:0.00} M";
        }

        private void StartPingCheck(BitfinexWebsocketClient client)
        {
            _pingSubscription = Observable
                .Interval(TimeSpan.FromSeconds(5))
                .Subscribe(async x =>
                {
                    _pingRequest = Stopwatch.StartNew();
                    await client.Send(new PingRequest());
                });      
        }

        private void HandlePong(PongResponse pong)
        {
            ComputePing(_pingRequest);
        }

        private void ComputePing(Stopwatch sw)
        {
            var elapsed = sw.ElapsedMilliseconds;
            _view.Ping = $"{elapsed:###} ms";
            _view.Status("Connected", StatusType.Info);
        }

        private void Clear()
        {
            _view.Bid = string.Empty;
            _view.Ask = string.Empty;
            _view.BidAmount = string.Empty;
            _view.AskAmount = string.Empty;
            _view.Trades1Min(string.Empty, Side.Buy);
            _view.Trades5Min(string.Empty, Side.Buy);
            _view.Trades15Min(string.Empty, Side.Buy);
            _view.Trades1Hour(string.Empty, Side.Buy);
            _view.Trades24Hours(string.Empty, Side.Buy);
        }
    }
}
