using System;
using Bitfinex.Client.Websocket.Json;
using Bitfinex.Client.Websocket.Requests.Configurations;
using Bitfinex.Client.Websocket.Responses.Configurations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Websocket.Client;

namespace Bitfinex.Client.Websocket.Client;

/// <summary>
/// Bitfinex websocket client, it wraps `IWebsocketClient` and parse raw data into streams.
/// </summary>
public abstract class BitfinexWebsocketClient<TStreams> : IDisposable where TStreams : BitfinexClientStreams, new()
{
    readonly IWebsocketClient _client;
    readonly IDisposable _messageReceivedSubscription;
    readonly IDisposable _configurationSubscription;

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="logger">The logger to use for logging any warnings or errors.</param>
    /// <param name="client">The client to use for the trade websocket.</param>
    protected BitfinexWebsocketClient(ILogger logger, IWebsocketClient client)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));

        _messageReceivedSubscription = _client.MessageReceived.Subscribe(HandleMessage);
        _configurationSubscription = Streams.ConfigurationStream.Subscribe(HandleConfiguration);
    }

    /// <summary>
    /// Logger.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// Provided message streams.
    /// </summary>
    public TStreams Streams { get; } = new();

    /// <summary>
    /// Currently enabled features
    /// </summary>
    public ConfigurationState Configuration { get; private set; } = new();

    /// <summary>
    /// Cleanup everything.
    /// </summary>
    public void Dispose()
    {
        _messageReceivedSubscription?.Dispose();
        _configurationSubscription?.Dispose();
    }

    /// <summary>
    /// Serializes request and sends message via websocket client. 
    /// It logs and re-throws every exception. 
    /// </summary>
    /// <param name="request">Request/message to be sent</param>
    public void Send<T>(T request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));

        try
        {
            var serialized = JsonConvert.SerializeObject(request, BitfinexJsonSerializer.Settings);
            _client.Send(serialized);
        }
        catch (Exception e)
        {
            Logger.LogError(e, LogMessage($"Exception while sending message '{request}'. Error: {e.Message}"));
            throw;
        }
    }

    void HandleConfiguration(ConfigurationResponse response)
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
                (flags & (int)ConfigurationFlag.Checksum) > 0);
        }
        catch (Exception e)
        {
            Logger.LogError(e, LogMessage("Exception while received configuration"));
        }
    }

    void HandleMessage(ResponseMessage message)
    {
        try
        {
            var formatted = (message.Text ?? string.Empty).Trim();

            if (formatted.StartsWith("{"))
            {
                HandleObjectMessage(formatted);
                return;
            }

            if (formatted.StartsWith("["))
            {
                var parsed = BitfinexSerialization.Deserialize<JArray>(formatted);
                if (parsed.Count < 2)
                {
                    LogWarning("Invalid message format, too few items");
                    return;
                }

                HandleArrayMessage(parsed);
                return;
            }

            LogWarning($"Unhandled response:  '{formatted}'");
        }
        catch (Exception e)
        {
            Logger.LogError(e, LogMessage("Exception while receiving message"));
        }
    }

    /// <summary>
    /// Formats a log message with a prefix.
    /// </summary>
    /// <param name="message">The message to format.</param>
    /// <returns>The formatted message.</returns>
    protected abstract string LogMessage(string message);

    /// <summary>
    /// Logs a warning message, formatted with a prefix.
    /// </summary>
    /// <param name="message">The message to format and log.</param>
    protected void LogWarning(string message) => Logger.LogWarning(LogMessage(message));

    /// <summary>
    /// Logs an error message, formatted with a prefix.
    /// </summary>
    /// <param name="message">The message to format and log.</param>
    protected void LogError(string message) => Logger.LogError(LogMessage(message));

    /// <summary>
    /// Handles an object message.
    /// </summary>
    /// <param name="message">The message to handle.</param>
    protected abstract void HandleObjectMessage(string message);

    /// <summary>
    /// Handles an array message.
    /// </summary>
    /// <param name="message">The message to handle.</param>
    protected abstract void HandleArrayMessage(JArray message);
}