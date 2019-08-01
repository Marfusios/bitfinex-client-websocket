using System;
using System.Reactive.Subjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Status
{
    /// <summary>
    /// Liquidation feed updates
    /// </summary>
    [JsonConverter(typeof(LiquidationFeedStatusConverter))]
    public class LiquidationFeedStatus : ResponseBase
    {
        /// <summary>
        /// ?
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Position id
        /// </summary>
        public long PosId { get; set; }

        /// <summary>
        /// Millisecond timestamp
        /// </summary>
        public DateTime TimestampMs { get; set; }

        /// <summary>
        /// Symbol
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Amount of the position
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// The price of the position.
        /// </summary>
        public double BasePrice { get; set; }

        /// <summary>
        /// 0 -> initial liquidation trigger, 1 market execution
        /// </summary>
        public int IsMatch { get; set; }

        /// <summary>
        /// 0 -> direct sell into the market, 1 position acquired by the system
        /// </summary>
        public int IsMarketSold { get; set; }

        internal static void Handle(JToken token, SubscribedResponse subscription,
            Subject<LiquidationFeedStatus> subject)
        {
            var data = token[1];

            if (data.Type != JTokenType.Array)
            {
                // probably heartbeat, ignore
                return;
            }

            // TODO: check if liquidation updates are always arrays
            if (data.First.Type == JTokenType.Array)
            {
                // initial snapshot
                var liquidations = data.ToObject<LiquidationFeedStatus[]>();
                foreach (var liquidation in liquidations)
                {
                    liquidation.ChanId = subscription.ChanId;
                    subject.OnNext(liquidation);
                }

                return;
            }

            var derivativePairStatus = data.ToObject<LiquidationFeedStatus>();
            derivativePairStatus.ChanId = subscription.ChanId;

            subject.OnNext(derivativePairStatus);
        }
    }
}