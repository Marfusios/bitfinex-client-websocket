using System.Diagnostics;
using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Margin
{
    [DebuggerDisplay("MarginInfo: {UserPl} - {MarginBalance} - {MarginNet}")]
    [JsonConverter(typeof(MarginInfoConverter))]
    public class MarginInfo
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// User Profit and Loss
        /// </summary>
        public double UserPl { get; set; }

        /// <summary>
        /// Amount of swaps a user has
        /// </summary>
        public double UserSwaps { get; set; }

        /// <summary>
        /// Balance in your margin funding account
        /// </summary>
        public double MarginBalance { get; set; }

        /// <summary>
        /// Balance after P/L is accounted for
        /// </summary>
        public double MarginNet { get; set; }

        /// <summary>
        /// Minimum required margin to keep positions open
        /// </summary>
        public double MarginRequired { get; set; }

        internal static void Handle(JToken token, Subject<MarginInfo> subject)
        {
            var data = token[2];
            if (data.Type != JTokenType.Array)
            {
                Log.Warn(L("MarginInfo - Invalid message format, third param not array"));
                return;
            }

            var parsed = data.ToObject<MarginInfo>();
            if (parsed != null)
            {
                subject.OnNext(parsed);
            }
        }

        private static string L(string msg)
        {
            return $"[BFX MARGIN INFO HANDLER] {msg}";
        }
    }
}