﻿using System;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bitfinex.Client.Websocket.Validations;
using Serilog;

namespace Bitfinex.Client.Websocket.Websockets
{
    public class BitfinexWebsocketCommunicator : IDisposable
    {
        private readonly Uri _url;
        private readonly Timer _lastChanceTimer;

        private DateTime _lastReceivedMsg = DateTime.UtcNow; 

        private bool _disposing = false;
        private ClientWebSocket _client;
        private CancellationTokenSource _cancelation;

        private readonly Subject<string> _messageReceivedSubject = new Subject<string>();


        public IObservable<string> MessageReceived => _messageReceivedSubject.AsObservable();

        public BitfinexWebsocketCommunicator(Uri url, ClientWebSocket websocketClient = null)
        {
            BfxValidations.ValidateInput(url, nameof(url));

            _url = url;
            _client = websocketClient;

            var minute = 1000 * 60;
            _lastChanceTimer = new Timer(async x => await LastChance(x), null, minute, minute);
        }

        public void Dispose()
        {
            _disposing = true;
            Log.Information(L("Disposing.."));
            _lastChanceTimer?.Dispose();
            _cancelation?.Cancel();
            _client?.Abort();
            _client?.Dispose();
            _cancelation?.Dispose();
        }

        public Task Start()
        {
            Log.Information(L("Starting.."));
            _cancelation = new CancellationTokenSource();

            return StartClient(_url, _cancelation.Token);
        }

        public async Task Send(string message)
        {
            BfxValidations.ValidateInput(message, nameof(message));

            Log.Debug(L($"Sending:  {message}"));
            var buffer = Encoding.UTF8.GetBytes(message);
            var messageSegment = new ArraySegment<byte>(buffer);
            var client = await GetClient();
            await client.SendAsync(messageSegment, WebSocketMessageType.Text, true, _cancelation.Token);
        }

        private async Task StartClient(Uri uri, CancellationToken token)
        {
            _client = _client ?? new ClientWebSocket() {Options = {KeepAliveInterval = new TimeSpan(0, 0, 0, 10)}};
            
            try
            {
                await _client.ConnectAsync(uri, token);
#pragma warning disable 4014
                Listen(_client, token);
#pragma warning restore 4014
            }
            catch (Exception e)
            {
                Log.Error(e, L("Exception while connecting"));
                await Reconnect();
            }
            
        }

        private async Task<ClientWebSocket> GetClient()
        {
            if (_client == null || (_client.State != WebSocketState.Open && _client.State != WebSocketState.Connecting))
            {
                await Reconnect();
            }
            return _client;
        }

        private async Task Reconnect()
        {
            if (_disposing)
                return;
            Log.Information(L("Reconnecting..."));
            _cancelation.Cancel();
            await Task.Delay(10000);

            _cancelation = new CancellationTokenSource();
            await StartClient(_url, _cancelation.Token);
        }

        private async Task Listen(ClientWebSocket client, CancellationToken token)
        {
            do
            {
                WebSocketReceiveResult result = null;
                var buffer = new byte[1000];
                var message = new ArraySegment<byte>(buffer);
                var resultMessage = new StringBuilder();
                do
                {
                    result = await client.ReceiveAsync(message, token);
                    var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    resultMessage.Append(receivedMessage);
                    if (result.MessageType != WebSocketMessageType.Text)
                        break;

                } while (!result.EndOfMessage);

                var received = resultMessage.ToString();
                Log.Debug(L($"Received:  {received}"));
                _lastReceivedMsg = DateTime.UtcNow;
                _messageReceivedSubject.OnNext(received);

            } while (client.State == WebSocketState.Open && !token.IsCancellationRequested);
        }

        private async Task LastChance(object state)
        {
            var diffMin = Math.Abs(DateTime.UtcNow.Subtract(_lastReceivedMsg).TotalMinutes);
            if(diffMin > 1)
                Log.Information(L($"Last message received {diffMin} min ago"));
            if (diffMin > 2)
            {
                Log.Information(L("Last message received more than 2 min ago. Hard restart.."));

                _client?.Abort();
                _client?.Dispose();
                _client = null;
                await Reconnect();
            }
        }

        private string L(string msg)
        {
            return $"[BFX WEBSOCKET COMMUNICATOR] {msg}";
        }
    }
}
