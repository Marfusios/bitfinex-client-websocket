using Bitfinex.Client.Websocket.Json;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Responses.Configurations;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Fundings
{
    /// <summary>
    /// The order that causes the trade determines if it is a buy or a sell.
    /// </summary>
    [JsonConverter(typeof(FundingConverter))]
    public class Funding : ResponseBase
    {
        public long Id { get; set; }

        /// <summary>
        /// Millisecond time stamp
        /// </summary>
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Mts { get; set; }

        /// <summary>
        /// How much was bought (positive) or sold (negative).
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Rate at which funding transaction occurred
        /// </summary>
        public double Rate { get; set; }

        /// <summary>
        /// Amount of time the funding transaction was for
        /// </summary>
        public double Period { get; set; }

        /// <summary>
        /// Type of the funding
        /// </summary>
        [JsonIgnore]
        public FundingType Type { get; set; }

        /// <summary>
        /// Target Symbol
        /// </summary>        
        [JsonIgnore]
        public string Symbol { get; set; }



        internal static void Handle(JToken token, SubscribedResponse subscription, 
            ConfigurationState config, Subject<Funding> subject)
        {
            var firstPosition = token[1];
            if (firstPosition.Type == JTokenType.Array)
            {
                // initial snapshot
                Handle(token, firstPosition.ToObject<Funding[]>(),subscription, config, subject);
                return;
            }

            var fundingType = FundingType.Executed;
            if (firstPosition.Type == JTokenType.String)
            {
                if ((string)firstPosition == "ftu")
                    fundingType = FundingType.UpdateExecution;
                else if ((string)firstPosition == "hb")
                    return; // heartbeat, ignore
            }

            var data = token[2];
            if (data.Type != JTokenType.Array)
            {
                // bad format, ignore
                return;
            }

            var funding = data.ToObject<Funding>();
            funding.Type = fundingType;
            funding.Symbol = subscription.Symbol;
            funding.ChanId = subscription.ChanId;
            SetGlobalData(funding, config, token, 2);
            subject.OnNext(funding);
        }

        internal static void Handle(JToken token, Funding[] fundings, SubscribedResponse subscription, 
            ConfigurationState config, Subject<Funding> subject)
        {
            var reversed = fundings.Reverse().ToArray(); // newest last
            foreach (var funding in reversed)
            {
                funding.Type = FundingType.Executed;
                funding.Symbol = subscription.Symbol;
                funding.ChanId = subscription.ChanId;
                SetGlobalData(funding, config, token);
                subject.OnNext(funding);
            }
        }

    }
    
}
