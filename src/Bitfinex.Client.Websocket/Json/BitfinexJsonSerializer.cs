using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Bitfinex.Client.Websocket.Json;

/// <summary>
/// Helper class for JSON serialization
/// </summary>
public static class BitfinexJsonSerializer
{
    /// <summary>
    /// Unified JSON settings
    /// </summary>
    public static readonly JsonSerializerSettings Settings = new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        Formatting = Formatting.None,
        Converters = new List<JsonConverter> { new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() } },
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    /// <summary>
    /// Custom preconfigured serializer
    /// </summary>
    public static readonly JsonSerializer Serializer = JsonSerializer.Create(Settings);

    internal static ILogger PublicLogger = NullLogger.Instance;
    internal static ILogger AuthenticatedLogger = NullLogger.Instance;
}