using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Responses;
using Bitfinex.Client.Websocket.Responses.Orders;
using Bitfinex.Client.Websocket.Responses.Tickers;
using Bitfinex.Client.Websocket.Responses.Trades;
using Bitfinex.Client.Websocket.Responses.Wallets;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Bitfinex.Client.Websocket.Client
{
    public class BitfinexClientStreams
    {
        private readonly Subject<ErrorResponse> _errorSubject = new Subject<ErrorResponse>();
        private readonly Subject<InfoResponse> _infoSubject = new Subject<InfoResponse>();
        private readonly Subject<PongResponse> _pongSubject = new Subject<PongResponse>();
        private readonly Subject<AuthenticationResponse> _authenticationSubject = new Subject<AuthenticationResponse>();
        private readonly Subject<Ticker> _tickerSubject = new Subject<Ticker>();
        private readonly Subject<Trade> _tradesSubject = new Subject<Trade>();

        private readonly Subject<Wallet[]> _walletsSubject = new Subject<Wallet[]>();
        private readonly Subject<Order[]> _ordersSubject = new Subject<Order[]>();
        private readonly Subject<Order> _orderCreatedSubject = new Subject<Order>();
        private readonly Subject<Order> _orderUpdatedSubject = new Subject<Order>();
        private readonly Subject<Order> _orderCanceledSubject = new Subject<Order>();

        public IObservable<ErrorResponse> ErrorStream => _errorSubject.AsObservable();
        public IObservable<InfoResponse> InfoStream => _infoSubject.AsObservable();
        public IObservable<PongResponse> PongStream => _pongSubject.AsObservable();
        public IObservable<AuthenticationResponse> AuthenticationStream => _authenticationSubject.AsObservable();
        public IObservable<Ticker> TickerStream => _tickerSubject.AsObservable();
        public IObservable<Trade> TradesStream => _tradesSubject.AsObservable();

        public IObservable<Wallet[]> WalletsStream => _walletsSubject.AsObservable();
        public IObservable<Order[]> OrdersStream => _ordersSubject.AsObservable();
        public IObservable<Order> OrderCreatedStream => _orderCreatedSubject.AsObservable();
        public IObservable<Order> OrderUpdatedStream => _orderUpdatedSubject.AsObservable();
        public IObservable<Order> OrderCanceledStream => _orderCanceledSubject.AsObservable();


        internal BitfinexClientStreams()
        {
            
        }

        internal void HandleAccountInfo(JToken token)
        {
            var itemsCount = token?.Count();
            if (token == null || itemsCount < 2)
            {
                Log.Warning($"Invalid message format, too low items");
                return;
            }

            var secondItem = token[1];
            if (secondItem.Type != JTokenType.String)
            {
                Log.Warning(L("Invalid message format, second param is not string"));
                return;
            }
            var msgType = (string)secondItem;
            if (msgType == "hb")
            {
                // hearbeat, ignore
                return;
            }

            if (itemsCount < 3)
            {
                Log.Warning(L("Invalid message format, too low items"));
                return;
            }

            switch (msgType)
            {
                case "ws":
                    HandleWalletsInfo(token);
                    break;
                case "os":
                    HandleOrdersInfo(token);
                    break;
                case "on":
                    HandleOrderCreate(token);
                    break;
                case "ou":
                    HandleOrderUpdate(token);
                    break;
                case "oc":
                    HandleOrderCancel(token);
                    break;
            }
        }

        internal void Raise(ErrorResponse response)
        {
            _errorSubject.OnNext(response);
        }

        internal void Raise(InfoResponse response)
        {
            _infoSubject.OnNext(response);
        }

        internal void Raise(PongResponse response)
        {
            _pongSubject.OnNext(response);
        }

        internal void Raise(Ticker response)
        {
            _tickerSubject.OnNext(response);
        }

        internal void Raise(Trade response)
        {
            _tradesSubject.OnNext(response);
        }

        internal void Raise(AuthenticationResponse response)
        {
            _authenticationSubject.OnNext(response);
        }

        private void HandleWalletsInfo(JToken token)
        {
            var data = token[2];
            if (data.Type != JTokenType.Array)
            {
                Log.Warning(L("Wallets - Invalid message format, third param not array"));
                return;
            }

            var parsed = data.ToObject<Wallet[]>();
            _walletsSubject.OnNext(parsed);
        }

        private void HandleOrdersInfo(JToken token)
        {
            var data = token[2];
            if (data.Type != JTokenType.Array)
            {
                Log.Warning(L("Orders - Invalid message format, third param not array"));
                return;
            }

            var parsed = data.ToObject<Order[]>();
            _ordersSubject.OnNext(parsed);
        }

        private void HandleOrderCreate(JToken token)
        {
            _orderCreatedSubject.OnNext(ParseOrderInfo(token));
        }

        private void HandleOrderUpdate(JToken token)
        {
            _orderUpdatedSubject.OnNext(ParseOrderInfo(token));
        }

        private void HandleOrderCancel(JToken token)
        {
            _orderCanceledSubject.OnNext(ParseOrderInfo(token));
        }

        private Order ParseOrderInfo(JToken token)
        {
            var data = token[2];
            if (data.Type != JTokenType.Array)
            {
                Log.Warning(L("Order info - Invalid message format, third param not array"));
                return null;
            }

            var parsed = data.ToObject<Order>();
            return parsed;
        }

        private string L(string msg)
        {
            return $"[BFX CLIENT STREAMS] {msg}";
        }
    }
}
