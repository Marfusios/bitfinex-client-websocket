using Bitfinex.Client.Websocket.Messages;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests.Configurations;

/// <summary>
/// Request to configure websocket connection
/// </summary>
public class ConfigurationRequest : RequestBase
{
    /// <summary>
    /// Request to configure websocket connection
    /// </summary>
    public ConfigurationRequest()
    {
    }

    /// <summary>
    /// Request to configure websocket connection
    /// </summary>
    /// <param name="flags">Flags - the bitwise XOR of the different options</param>
    public ConfigurationRequest(ConfigurationFlag flags)
    {
        SelectedFlags = flags;
    }

    /// <inheritdoc />
    public override MessageType EventType => MessageType.Conf;

    /// <summary>
    /// Flags - the bitwise XOR of the different options
    /// 
    /// Example:
    /// To enable all decimals as strings and all times as date strings, you would set the value of flags to 40
    /// </summary>
    [JsonIgnore]
    public ConfigurationFlag SelectedFlags { get; set; }

    /// <summary>
    /// Flags - converted to integer
    /// </summary>
    public int Flags => (int)SelectedFlags;
}