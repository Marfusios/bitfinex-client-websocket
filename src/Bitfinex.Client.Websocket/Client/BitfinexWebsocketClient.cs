using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bitfinex.Client.Websocket.Communicator;
using Bitfinex.Client.Websocket.Json;
using Bitfinex.Client.Websocket.Requests;
using Bitfinex.Client.Websocket.Validations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

using static Bitfinex.Client.Websocket.Client.BitfinexLogger;

namespace Bitfinex.Client.Websocket.Client
{
    /// <summary>
    /// Bitfinex websocket client, it wraps `IBitfinexCommunicator` and parse raw data into streams.
    /// Send subscription requests (for example: `new TradesSubscribeRequest(pair)`) and subscribe to `Streams`
    /// </summary>
    public class BitfinexWebsocketClient : IDisposable
    {
        private readonly IBitfinexCommunicator _communicator;
        private readonly IDisposable _messageReceivedSubscription;

        private readonly Dictionary<int, Action<JToken>> _channelIdToHandler = new Dictionary<int, Action<JToken>>();

        private readonly BitfinexAuthenticatedHandler _authenticatedHandler;
        private readonly BitfinexPublicHandler _publicHandler;

        /// <inheritdoc />
        public BitfinexWebsocketClient(IBitfinexCommunicator communicator)
        {
            BfxValidations.ValidateInput(communicator, nameof(communicator));

            _communicator = communicator;
            _messageReceivedSubscription = _communicator.MessageReceived.Subscribe(HandleMessage);

            _authenticatedHandler = new BitfinexAuthenticatedHandler(Streams, _channelIdToHandler);
            _publicHandler = new BitfinexPublicHandler(Streams, _channelIdToHandler);
        }

        /// <summary>
        /// Provided message streams
        /// </summary>
        public BitfinexClientStreams Streams { get; } = new BitfinexClientStreams();

        /// <summary>
        /// Cleanup everything
        /// </summary>
        public void Dispose()
        {
            _messageReceivedSubscription?.Dispose();
        }

        /// <summary>
        /// Serializes request and sends message via websocket communicator. 
        /// It logs and re-throws every exception. 
        /// </summary>
        /// <param name="request">Request/message to be sent</param>
        public async Task Send<T>(T request)
        {
            try
            {
                BfxValidations.ValidateInput(request, nameof(request));

                var serialized = JsonConvert.SerializeObject(request, BitfinexJsonSerializer.Settings);
                await _communicator.Send(serialized).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Log.Error(e, L($"Exception while sending message '{request}'. Error: {e.Message}"));
                throw;
            }
        }

        /// <summary>
        /// Sends authentication request via websocket communicator
        /// </summary>
        /// <param name="apiKey">Your API key</param>
        /// <param name="apiSecret">Your API secret</param>
        /// <param name="deadManSwitchEnabled">Dead-Man-Switch flag (optional), when socket is closed, cancel all account orders</param>
        public Task Authenticate(string apiKey, string apiSecret, bool deadManSwitchEnabled = false)
        {
            return Send(new AuthenticationRequest(apiKey, apiSecret, deadManSwitchEnabled));
        }

        private void HandleMessage(string message)
        {
            try
            {
                var formatted = (message ?? string.Empty).Trim();

                if (formatted.StartsWith("{"))
                {
                    _publicHandler.OnObjectMessage(formatted);
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

        private void OnArrayMessage(string msg)
        {
            var parsed = BitfinexSerialization.Deserialize<JArray>(msg);
            if (parsed.Count() < 2)
            {
                Log.Warning(L("Invalid message format, too low items"));
                return;
            }

            var channelId = (int)parsed[0];

            if (!_channelIdToHandler.ContainsKey(channelId))
            {
                Log.Debug($"Unrecognized channel id '{channelId}', ignoring..");
                return;
            }

            _channelIdToHandler[channelId](parsed);
        }
    }
}
