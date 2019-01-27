using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Responses.Configurations;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses
{
    /// <summary>
    /// Response with checksum value
    /// </summary>
    public class ChecksumResponse : ResponseBase
    {
        /// <summary>
        /// Checksum value
        /// </summary>
        public int Checksum { get; set; }

        /// <summary>
        /// Target pair
        /// </summary>
        public string Pair { get; set; }


        internal static void Handle(JToken token, SubscribedResponse subscription, ConfigurationState config, Subject<ChecksumResponse> subject)
        {
            var checksum = token[2].Value<int>();
            var response = new ChecksumResponse()
            {
                Checksum = checksum,
                Pair = subscription.Pair,
                ChanId = subscription.ChanId
            };
            SetGlobalData(response, config, token, 2);
            subject.OnNext(response);
        }
    }
}
