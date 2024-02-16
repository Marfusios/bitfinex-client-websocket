using System;
using System.Linq;
using Bitfinex.Client.Websocket.Communicator;
using Bitfinex.Client.Websocket.Json;
using Bitfinex.Client.Websocket.Requests;
using Bitfinex.Client.Websocket.Requests.Configurations;
using Bitfinex.Client.Websocket.Responses.Configurations;
using Bitfinex.Client.Websocket.Validations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Websocket.Client;
using static Bitfinex.Client.Websocket.Client.BitfinexLogger;

namespace Bitfinex.Client.Websocket.Client
{
    /// <summary>
    /// Bitfinex websocket client, it wraps `IBitfinexCommunicator` and parse raw data into streams.
    /// Send subscription requests (for example: `new TradesSubscribeRequest(pair)`) and subscribe to `Streams`
    /// </summary>
    public class BitfinexWebsocketClient : IDisposable
    {
        private readonly ILogger<BitfinexWebsocketClient> _logger;
        private readonly IBitfinexCommunicator _communicator;
        private readonly IDisposable _messageReceivedSubscription;
        private readonly IDisposable _configurationSubscription;

        private readonly BitfinexChannelList _channelIdToHandler = new BitfinexChannelList();

        private readonly BitfinexAuthenticatedHandler _authenticatedHandler;
        private readonly BitfinexPublicHandler _publicHandler;

        /// <inheritdoc />
        public BitfinexWebsocketClient(IBitfinexCommunicator communicator, ILogger<BitfinexWebsocketClient>? logger = null)
        {
            BfxValidations.ValidateInput(communicator, nameof(communicator));

            _communicator = communicator;
            _logger = logger ?? NullLogger<BitfinexWebsocketClient>.Instance;
            _messageReceivedSubscription = _communicator.MessageReceived.Subscribe(HandleMessage);
            _configurationSubscription = Streams.ConfigurationSubject.Subscribe(HandleConfiguration);

            _authenticatedHandler = new BitfinexAuthenticatedHandler(Streams, _channelIdToHandler, _logger);
            _publicHandler = new BitfinexPublicHandler(Streams, _channelIdToHandler);
        }

        /// <summary>
        /// Provided message streams
        /// </summary>
        public BitfinexClientStreams Streams { get; } = new BitfinexClientStreams();

        /// <summary>
        /// Current enabled features
        /// </summary>
        public ConfigurationState Configuration { get; private set; } = new ConfigurationState();

        /// <summary>
        /// Expose logger for this client
        /// </summary>
        public ILogger<BitfinexWebsocketClient> Logger => _logger;

        /// <summary>
        /// Cleanup everything
        /// </summary>
        public void Dispose()
        {
            _messageReceivedSubscription?.Dispose();
            _configurationSubscription?.Dispose();
        }

        /// <summary>
        /// Serializes request and sends message via websocket communicator. 
        /// It logs and re-throws every exception. 
        /// </summary>
        /// <param name="request">Request/message to be sent</param>
        public void Send<T>(T request)
        {
            try
            {
                BfxValidations.ValidateInput(request, nameof(request));

                var serialized = JsonConvert.SerializeObject(request, BitfinexJsonSerializer.Settings);
                _communicator.Send(serialized);
            }
            catch (Exception e)
            {
                _logger.LogError(e, L("Exception while sending message '{request}'. Error: {error}"), request, e.Message);
                throw;
            }
        }

        /// <summary>
        /// Sends authentication request via websocket communicator
        /// </summary>
        /// <param name="apiKey">Your API key</param>
        /// <param name="apiSecret">Your API secret</param>
        /// <param name="deadManSwitchEnabled">Dead-Man-Switch flag (optional), when socket is closed, cancel all account orders</param>
        public void Authenticate(string apiKey, string apiSecret, bool deadManSwitchEnabled = false)
        {
            Send(new AuthenticationRequest(apiKey, apiSecret, deadManSwitchEnabled));
        }

        private void HandleConfiguration(ConfigurationResponse response)
        {
            try
            {
                if (!response.IsConfigured || !response.Flags.HasValue)
                {
                    Configuration = new ConfigurationState();
                    return;
                }

                var flags = response.Flags.Value;
                Configuration = new ConfigurationState(
                    (flags & (int)ConfigurationFlag.DecimalAsString) > 0,
                    (flags & (int)ConfigurationFlag.TimeAsString) > 0,
                    (flags & (int)ConfigurationFlag.Timestamp) > 0,
                    (flags & (int)ConfigurationFlag.Sequencing) > 0,
                    (flags & (int)ConfigurationFlag.Checksum) > 0
                    );
            }
            catch (Exception e)
            {
                _logger.LogError(e, L("Exception while received configuration, error: {error}"), e.Message);
            }
        }

        private void HandleMessage(ResponseMessage message)
        {
            try
            {
                var formatted = (message.Text ?? string.Empty).Trim();

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
                _logger.LogError(e, L("Exception while receiving message, error: {error}"), e.Message);
            }
        }

        private void OnArrayMessage(string msg)
        {
            var parsed = BitfinexSerialization.Deserialize<JArray>(msg);
            if (parsed.Count() < 2)
            {
                _logger.LogWarning(L("Invalid message format, too low items"));
                return;
            }

            var channelId = (int)parsed[0];

            if (!_channelIdToHandler.ContainsKey(channelId))
            {
                _logger.LogDebug("Unrecognized channel id '{channelId}', ignoring..", channelId);
                return;
            }

            _channelIdToHandler[channelId](parsed, Configuration);
        }
    }
}
