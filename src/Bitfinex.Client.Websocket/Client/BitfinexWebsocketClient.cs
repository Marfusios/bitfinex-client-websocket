using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bitfinex.Client.Websocket.Json;
using Bitfinex.Client.Websocket.Messages;
using Bitfinex.Client.Websocket.Requests;
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

        public Task Send<T>(T request)
        {
            BfxValidations.ValidateInput(request, nameof(request));

            var serialized = JsonConvert.SerializeObject(request, BitfinexJsonSerializer.Settings);
            return _communicator.Send(serialized);
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
            if(!response.IsAuthenticated)
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
            Streams.Raise(ticker);
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
