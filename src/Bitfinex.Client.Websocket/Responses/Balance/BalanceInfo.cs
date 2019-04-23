using System.Diagnostics;
using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Balance
{
    /// <summary>
    /// Balance response
    /// </summary>
    [DebuggerDisplay("BalanceInfo {TotalAum} {NetAum}")]
    [JsonConverter(typeof(BalanceInfoConverter))]
    public class BalanceInfo
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        /// <summary>
        /// Total Assets Under Management
        /// </summary>
        public double TotalAum { get; set; }

        /// <summary>
        /// Net Assets Under Management
        /// </summary>
        public double NetAum { get; set; }

        internal static void Handle(JToken token, Subject<BalanceInfo> subject)
        {
            var data = token[2];
            if (data.Type != JTokenType.Array)
            {
                Log.Warn(L("BalanceInfo - Invalid message format, third param not array"));
                return;
            }

            var parsed = data.ToObject<BalanceInfo>();
            subject.OnNext(parsed);
        }

        private static string L(string msg)
        {
            return $"[BFX BALANCE INFO HANDLER] {msg}";
        }
    }
}