using System.Diagnostics;
using System.Linq;
using System.Reactive.Subjects;
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
            if (data?.Type != JTokenType.Array)
            {
                return;
            }

            var parsed = data.ToObject<Wallet[]>();
            if (parsed != null)
            {
                subjectMultiple.OnNext(parsed);
                parsed.ToList().ForEach(subject.OnNext);
            }
        }

        internal static void Handle(JToken token, Subject<Wallet> subject)
        {
            var data = token[2];
            if (data?.Type != JTokenType.Array)
            {
                return;
            }

            var parsed = data.ToObject<Wallet>();
            if (parsed != null)
                subject.OnNext(parsed);
        }
    }
}
