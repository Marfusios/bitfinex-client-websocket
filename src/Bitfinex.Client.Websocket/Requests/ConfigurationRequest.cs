using Bitfinex.Client.Websocket.Messages;

namespace Bitfinex.Client.Websocket.Requests
{
    /// <summary>
    /// Request to configure websocket connection
    /// </summary>
    public class ConfigurationRequest : RequestBase
    {
        /// <inheritdoc />
        public override MessageType EventType => MessageType.Conf;

        /// <summary>
        /// Flags - the bitwise XOR of the different options
        /// 
        /// Example:
        /// To enable all decimals as strings and all times as date strings, you would set the value of flags to 40
        /// </summary>
        public ConfigurationFlag Flags { get; set; }
    }
}
