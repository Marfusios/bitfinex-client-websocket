using System;
using Bitfinex.Client.Websocket.Responses.Configurations;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Bitfinex.Client.Websocket.Responses
{
    /// <summary>
    /// Base class for every response
    /// </summary>
    public class ResponseBase
    {
        /// <summary>
        /// Unique channel id for this type of response.
        /// </summary>
        public int ChanId { get; set; }

        /// <summary>
        /// Server unique sequence number (in most cases not needed, because of TCP/IP delivery guarantees). 
        /// Sequencing must be enabled by configuration (see `ConfigurationRequest`). 
        /// </summary>
        public long? ServerSequence { get; set; }

        /// <summary>
        /// Server timestamp. 
        /// Timestamp must be enabled by configuration (see `ConfigurationRequest`). 
        /// </summary>
        public DateTime? ServerTimestamp { get; set; }

        /// <summary>
        /// Sets global data (timestamp, sequence) if it is enabled by configuration.
        /// </summary>
        public static void SetGlobalData(ResponseBase response, ConfigurationState config, JToken data, int lastPosition = 1)
        {
            try
            {
                var position = lastPosition + 1;

                if (config.IsSequencingEnabled)
                {
                    var sequence = data[position].Value<long?>();
                    response.ServerSequence = sequence;
                    position++;
                }

                if (config.IsTimestampEnabled)
                {
                    var mts = data[position].Value<long?>();
                    response.ServerTimestamp = BitfinexTime.ConvertToTime(mts);
                    // position++;
                }
            }
            catch (Exception e)
            {
                Log.Warning($"[BFX response] Failed to parse global data (timestamp, sequence). Error: {e.Message}");
            }
        }
    }
}
