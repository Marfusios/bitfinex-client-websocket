using System;

namespace Bitfinex.Client.Websocket.Requests
{
    /// <summary>
    /// In order to change the configuration, there is a new event able to be requested conf, and this will have a parameter flags which is the bitwise XOR of the different options listed below
    /// 
    /// Example:
    /// To enable all decimals as strings and all times as date strings, you would set the value of flags to 40
    /// </summary>
    [Flags]
    public enum ConfigurationFlag
    {
        /// <summary>
        /// None
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Enable all decimal as strings.
        /// </summary>
        DecimalAsString = 8,

        /// <summary>
        /// Enable all times as date strings.
        /// </summary>
        TimeAsString = 32,

        /// <summary>
        /// Timestamp in milliseconds.
        /// </summary>
        Timestamp = 32768,

        /// <summary>
        /// Enable sequencing BETA FEATURE
        /// </summary>
        Sequencing = 65536,

        /// <summary>
        /// Enable checksum for every book iteration. Checks the top 25 entries for each side of book. Checksum is a signed int.
        /// </summary>
        Checksum = 131072
    }
}
