using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bitfinex.Client.Websocket.Json;
using Bitfinex.Client.Websocket.Messages;
using Bitfinex.Client.Websocket.Requests;
using Bitfinex.Client.Websocket.Responses;
using Bitfinex.Client.Websocket.Responses.Books;
using Bitfinex.Client.Websocket.Responses.Candles;
using Bitfinex.Client.Websocket.Responses.Tickers;
using Bitfinex.Client.Websocket.Responses.Trades;
using Bitfinex.Client.Websocket.Utils;
using Bitfinex.Client.Websocket.Validations;
using Bitfinex.Client.Websocket.Websockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Bitfinex.Client.Websocket.Client
{
    public class BitfinexWebsocketClient : IDisposable
    {
        private readonly BitfinexWebsocketCommunicator _communicator;
        private readonly IDisposable _messageReceivedSubsciption;

        private readonly Dictionary<int, Action<JToken>> _channelIdToHandler = new Dictionary<int, Action<JToken>>();

        public BitfinexWebsocketClient(BitfinexWebsocketCommunicator communicator)
        {
            BfxValidations.ValidateInput(communicator, nameof(communicator));

            _communicator = communicator;
            _channelIdToHandler[0] = Streams.HandleAccountInfo;
            _messageReceivedSubsciption = _communicator.MessageReceived.Subscribe(HandleMessage);
        }

        public BitfinexClientStreams Streams { get; } = new BitfinexClientStreams();

        public void Dispose()
        {
            _messageReceivedSubsciption?.Dispose();
        }

        public async Task Send<T>(T request)
        {
            try
            {
                BfxValidations.ValidateInput(request, nameof(request));

                var serialized = JsonConvert.SerializeObject(request, BitfinexJsonSerializer.Settings);
                await _communicator.Send(serialized);
            }
            catch (Exception e)
            {
                Log.Error(e, L($"Exception while sending message '{request}'. Error: {e.Message}"));
                throw;
            }
        }

        public Task Authenticate(string apiKey, string apiSecret)
        {
            return Send(new AuthenticationRequest(apiKey, apiSecret));
        }



        private void HandleMessage(string message)
        {
            try
            {
                var formatted = (message ?? string.Empty).Trim();

                if (formatted.StartsWith("{"))
                {
                    OnObjectMessage(formatted);
                    return;
                }

                if (formatted.StartsWith("["))
                {
                    OnArrayMessage(formatted);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error(e, L("Exception while receiving message"));
            }
        }

        private void OnObjectMessage(string msg)
        {
            var parsed = Deserialize<MessageBase>(msg);

            switch (parsed.Event)
            {
                case MessageType.Error:
                    OnError(Deserialize<ErrorResponse>(msg));
                    break;
                case MessageType.Info:
                    Streams.Raise(Deserialize<InfoResponse>(msg));
                    break;
                case MessageType.Auth:
                    OnAuthentication(Deserialize<AuthenticationResponse>(msg));
                    break;
                case MessageType.Pong:
                    Streams.Raise(Deserialize<PongResponse>(msg));
                    break;
                case MessageType.Subscribed:
                    OnSubscription(Deserialize<SubscribedResponse>(msg));
                    break;
                default:
                    break;
            }
        }

        private void OnError(ErrorResponse error)
        {
            Log.Error(L($"Error received - message: {error.Msg}, code: {error.Code}"));
            Streams.Raise(error);
        }

        private void OnAuthentication(AuthenticationResponse response)
        {
            if (!response.IsAuthenticated)
                Log.Warning(L("Authentication failed. Code: " + response.Code));
            Streams.Raise(response);
        }

        private void OnArrayMessage(string msg)
        {
            var parsed = Deserialize<JArray>(msg);
            if (parsed.Count() < 2)
            {
                Log.Warning(L("Invalid message format, too low items"));
                return;
            }

            var channelId = (int)parsed[0];

            if (!_channelIdToHandler.ContainsKey(channelId))
                return;

            _channelIdToHandler[channelId](parsed);
        }

        private void OnSubscription(SubscribedResponse response)
        {
            var channelId = response.ChanId;

            switch (response.Channel)
            {
                case "ticker":
                    _channelIdToHandler[channelId] = data => OnTicker(data, response);
                    break;
                case "trades":
                    _channelIdToHandler[channelId] = data => OnTrades(data, response);
                    break;
                case "candles":
                    _channelIdToHandler[channelId] = data => OnCandles(data, response);
                    break;
                case "book":
                    _channelIdToHandler[channelId] = data => OnBook(data, response);
                    break;
            }
        }

        private void OnTicker(JToken token, SubscribedResponse subscription)
        {
            var data = token[1];

            if (data.Type != JTokenType.Array)
            {
                // probably heartbeat, ignore
                return;
            }

            var ticker = data.ToObject<Ticker>();
            ticker.Pair = subscription.Pair;
            ticker.ChanId = subscription.ChanId;
            Streams.Raise(ticker);
        }

        private void OnTrades(JToken token, SubscribedResponse subscription)
        {
            var firstPosition = token[1];
            if (firstPosition.Type == JTokenType.Array)
            {
                // initial snapshot
                OnTrades(firstPosition.ToObject<Trade[]>(), subscription);
                return;
            }

            var tradeType = TradeType.Executed;
            if (firstPosition.Type == JTokenType.String)
            {
                if((string)firstPosition == "tu")
                    tradeType = TradeType.UpdateExecution;
                else if((string)firstPosition == "hb")
                    return; // heartbeat, ignore
            }

            var data = token[2];
            if (data.Type != JTokenType.Array)
            {
                // bad format, ignore
                return; 
            }

            var trade = data.ToObject<Trade>();
            trade.Type = tradeType;
            trade.Pair = subscription.Pair;
            trade.ChanId = subscription.ChanId;
            Streams.Raise(trade);
        }

        private void OnTrades(Trade[] trades, SubscribedResponse subscription)
        {
            var reversed = trades.Reverse().ToArray(); // newest last
            foreach (var trade in reversed)
            {
                trade.Type = TradeType.Executed;
                trade.Pair = subscription.Pair;
                trade.ChanId = subscription.ChanId;
                Streams.Raise(trade);
            }
        }

        private void OnCandles(JToken token, SubscribedResponse subscription)
        {
            var data = token[1];

            if (data.Type != JTokenType.Array)
            {
                // probably heartbeat, ignore
                return;
            }

            var candles = data.ToObject<Candles>();

            candles.TimeFrame = new BitfinexTimeFrame().GetFieldByStringValue(subscription.Key.Split(':')[1]);
            candles.Pair = subscription.Key.Split(':')[2].Remove(0, 1);
            candles.ChanId = subscription.ChanId;
            Streams.Raise(candles);
        }

        private void OnBook(JToken token, SubscribedResponse subscription)
        {
            var data = token[1];

            if (data.Type != JTokenType.Array)
            {
                return; // heartbeat, ignore
            }

            if(data.First.Type == JTokenType.Array)
            {
                // initial snapshot
                OnBooks(data.ToObject<Book[]>(), subscription);
                return;
            }

            var book = data.ToObject<Book>();
            book.Pair = subscription.Pair;
            book.ChanId = subscription.ChanId;
            Streams.Raise(book);
        }

        private void OnBooks(Book[] books, SubscribedResponse subscription)
        {
            //var reversed = books.Reverse().ToArray(); // newest last
            foreach (var book in books)
            {
                book.Pair = subscription.Pair;
                book.ChanId = subscription.ChanId;
                Streams.Raise(book);
            }
        }

        private T Deserialize<T>(string msg)
        {
            return JsonConvert.DeserializeObject<T>(msg, BitfinexJsonSerializer.Settings);
        }

        private string L(string msg)
        {
            return $"[BFX WEBSOCKET CLIENT] {msg}";
        }
    }
}
