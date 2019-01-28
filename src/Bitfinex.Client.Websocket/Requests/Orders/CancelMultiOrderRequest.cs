using Bitfinex.Client.Websocket.Requests.Converters;
using Bitfinex.Client.Websocket.Requests.Orders;
using Bitfinex.Client.Websocket.Validations;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests
{
    /// <summary>
    /// Cancel multiple orders at once.
    /// You can cancel the order by in a few different ways:
    /// Cancel 'All' open orders,
    /// Using the Internal Order IDs,
    /// Using the Client Order ID (supplied by you). The Client Order ID is unique per day,
    /// so you also have to provide the date of the order as a date string in this format YYYY-MM-DD.
    /// Using the Group Order ID.
    /// </summary>
    [JsonConverter(typeof(CancelMultiOrderConverter))]
    public class CancelMultiOrderRequest
    {
        /// <summary>
        /// Cancel all active orders
        /// </summary>
        public CancelMultiOrderRequest(bool cancelAll)
        {
            CancelAll = cancelAll;
        }

        /// <summary>
        /// Cancel multiple orders by unique Bitfinex order ids
        /// </summary>
        /// <param name="ids"></param>
        public CancelMultiOrderRequest(long[] ids)
        {
            BfxValidations.ValidateInputCollection(ids, nameof(ids));

            Ids = ids;
        }

        /// <summary>
        /// Cancel multiple orders by unique client order identifications 
        /// </summary>
        /// <param name="cidPairs"></param>
        public CancelMultiOrderRequest(CidPair[] cidPairs)
        {
            BfxValidations.ValidateInputCollection(cidPairs, nameof(cidPairs));

            CidPairs = cidPairs;
        }

        private CancelMultiOrderRequest()
        {
        }

        /// <summary>
        /// Set true if you want to cancel all active orders
        /// </summary>
        public bool CancelAll { get; private set; }

        /// <summary>
        ///  Unique Bitfinex order ids (not cid)
        /// </summary>
        public long[] Ids { get; private set; }

        /// <summary>
        /// Unique client order identifications
        /// </summary>
        public CidPair[] CidPairs { get; private set; }

        /// <summary>
        /// Unique client group ids
        /// </summary>
        public long[] Gids { get; private set; }


        /// <summary>
        /// Cancel all orders
        /// </summary>
        public static CancelMultiOrderRequest CancelEverything()
        {
            return new CancelMultiOrderRequest()
            {
                CancelAll = true
            };
        }

        /// <summary>
        /// Cancel multiple order in selected groups
        /// </summary>
        public static CancelMultiOrderRequest CancelGroups(long[] gids)
        {
            BfxValidations.ValidateInputCollection(gids, nameof(gids));

            return new CancelMultiOrderRequest()
            {
                 Gids = gids
            };
        }

        /// <summary>
        /// Cancel multiple order in selected group
        /// </summary>
        public static CancelMultiOrderRequest CancelGroup(long gid)
        {
            BfxValidations.ValidateInput(gid, nameof(gid), 0);

            return new CancelMultiOrderRequest()
            {
                Gids = new []{gid}
            };
        }

        /// <summary>
        /// Cancel multiple orders by your custom combinations of unique ids, client ids or/and group ids
        /// </summary>
        public static CancelMultiOrderRequest CancelCombination(long[] ids, CidPair[] cidPairs, long[] gids)
        {
            return new CancelMultiOrderRequest()
            {
                Ids = ids,
                CidPairs = cidPairs,
                Gids = gids
            };
        }
    }
}
