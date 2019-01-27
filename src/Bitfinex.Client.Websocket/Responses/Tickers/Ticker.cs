using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Responses.Configurations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Tickers
{
    /// <summary>
    /// Current price (bid, ask) and statistics for target pair
    /// </summary>
    [JsonConverter(typeof(TickerConverter))]
    public class Ticker : ResponseBase
    {
        /// <summary>
        /// Price of last highest bid
        /// </summary>
        public double Bid { get; set; }

        /// <summary>
        /// Size of the last highest bid
        /// </summary>
        public double BidSize { get; set; }

        /// <summary>
        /// Price of last lowest ask
        /// </summary>
        public double Ask { get; set; }

        /// <summary>
        /// Size of the last lowest ask
        /// </summary>
        public double AskSize { get; set; }

        /// <summary>
        /// Amount that the last price has changed since yesterday
        /// </summary>
        public double DailyChange { get; set; }

        /// <summary>
        /// Amount that the price has changed expressed in percentage terms
        /// </summary>
        public double DailyChangePercent { get; set; }

        /// <summary>
        /// Price of the last trade
        /// </summary>
        public double LastPrice { get; set; }

        /// <summary>
        /// Daily volume
        /// </summary>
        public double Volume { get; set; }

        /// <summary>
        /// Daily high
        /// </summary>
        public double High { get; set; }

        /// <summary>
        /// Daily low
        /// </summary>
        public double Low { get; set; }

        /// <summary>
        /// Target pair
        /// </summary>
        [JsonIgnore]
        public string Pair { get; set; }


        internal static void Handle(JToken token, SubscribedResponse subscription, ConfigurationState config, Subject<Ticker> subject)
        {
            var data = token[1];

            if (data.Type != JTokenType.Array)
            {
                // probably heartbeat, ignore
                return;
            }

            var ticker = data.ToObject<Ticker>();
            ticker.Pair = subscription.Pair;
            ticker.ChanId = subscription.ChanId;
            SetGlobalData(ticker, config, token);
            subject.OnNext(ticker);
        }
    }
}