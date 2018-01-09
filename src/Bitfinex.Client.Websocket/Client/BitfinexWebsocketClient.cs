using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Bitfinex.Client.Websocket.Exceptions;
using Bitfinex.Client.Websocket.Json;
using Bitfinex.Client.Websocket.Messages;
using Bitfinex.Client.Websocket.Responses;
using Bitfinex.Client.Websocket.Responses.Tickers;
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

        private readonly Subject<ErrorResponse> _errorSubject = new Subject<ErrorResponse>();
        private readonly Subject<InfoResponse> _infoSubject = new Subject<InfoResponse>();
        private readonly Subject<PongResponse> _pongSubject = new Subject<PongResponse>();
        private readonly Subject<Ticker> _tickerSubject = new Subject<Ticker>();

        public IObservable<ErrorResponse> ErrorStream => _errorSubject.AsObservable();
        public IObservable<InfoResponse> InfoStream => _infoSubject.AsObservable();
        public IObservable<PongResponse> PongStream => _pongSubject.AsObservable();
        public IObservable<Ticker> TickerStream => _tickerSubject.AsObservable();

        public BitfinexWebsocketClient(BitfinexWebsocketCommunicator communicator)
        {
            BfxValidations.ValidateInput(communicator, nameof(communicator));

            _communicator = communicator;
            _messageReceivedSubsciption = _communicator.MessageReceived.Subscribe(async msg => await HandleMessage(msg));
        }

        public void Dispose()
        {
            _messageReceivedSubsciption?.Dispose();
        }

        public Task Send<T>(T request)
        {
            BfxValidations.ValidateInput(request, nameof(request));

            var serialized = JsonConvert.SerializeObject(request, BitfinexJsonSerializer.Settings);
            return _communicator.Send(serialized);
        }

        private async Task HandleMessage(string message)
        {
            try
            {
                var formatted = (message ?? string.Empty).Trim();

                if (formatted.StartsWith("{"))
                {
                    await OnObjectMessage(formatted);
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

        private async Task OnObjectMessage(string msg)
        {
            var parsed = Deserialize<MessageBase>(msg);

            switch (parsed.Event)
            {
                case MessageType.Error:
                    OnError(Deserialize<ErrorResponse>(msg));
                    break;
                case MessageType.Info:
                    OnInfo(Deserialize<InfoResponse>(msg));
                    break;
                case MessageType.Auth:
                    await OnAuthentication(Deserialize<AuthenticationResponse>(msg));
                    break;
                case MessageType.Pong:
                    _pongSubject.OnNext(Deserialize<PongResponse>(msg));
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
            _errorSubject.OnNext(error);
        }

        private void OnInfo(InfoResponse info)
        {
            _infoSubject.OnNext(info);
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

        private Task OnAuthentication(AuthenticationResponse response)
        {
            if(response.Status != "OK")
                throw new BitfinexException("Authentication failed. Code: " + response.Code);
            return Task.CompletedTask;
        }

        private void OnSubscription(SubscribedResponse response)
        {
            var channelId = response.ChanId;

            switch (response.Channel)
            {
                case "ticker":
                    _channelIdToHandler[channelId] = data => OnTicker(data, response);
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
            _tickerSubject.OnNext(ticker);
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
