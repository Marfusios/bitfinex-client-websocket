using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Wallets
{
    /// <summary>
    /// Wallet info, displays current balance of the currency.
    /// </summary>
    [DebuggerDisplay("Wallet {Type}: {Currency} {Balance}")]
    [JsonConverter(typeof(WalletConverter))]
    public class Wallet
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger(); 

        /// <summary>
        /// Wallet name (exchange, margin, funding)
        /// </summary>
        public WalletType Type { get; set; }

        /// <summary>
        /// Currency (fUSD, etc)
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Wallet balance
        /// </summary>
        public double Balance { get; set; }

        /// <summary>
        /// Unsettled interest
        /// </summary>
        public double UnsettledInterest { get; set; }

        /// <summary>
        /// Amount not tied up in active orders, positions or funding (null if the value is not fresh enough).
        /// In order to receive those values the user have to actively request for it with a "calc" message.
        /// </summary>
        public double? BalanceAvailable { get; set; }

        
        internal static void Handle(JToken token, Subject<Wallet> subject, Subject<Wallet[]> subjectMultiple)
        {
            var data = token[2];
            if (data.Type != JTokenType.Array)
            {
                Log.Warn(L("Wallets - Invalid message format, third param not array"));
                return;
            }

            var parsed = data.ToObject<Wallet[]>();
            subjectMultiple.OnNext(parsed);
            parsed.ToList().ForEach(subject.OnNext);
        }

        internal static void Handle(JToken token, Subject<Wallet> subject)
        {
            var data = token[2];
            if (data.Type != JTokenType.Array)
            {
                Log.Warn(L("Wallet update - Invalid message format, third param not array"));
                return;
            }

            var parsed = data.ToObject<Wallet>();
            subject.OnNext(parsed);
        }

        private static string L(string msg)
        {
            return $"[BFX WALLET HANDLER] {msg}";
        }
    }
}
