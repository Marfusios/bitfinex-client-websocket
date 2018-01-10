using Bitfinex.Client.Websocket.Requests.Converters;
using Bitfinex.Client.Websocket.Validations;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests
{
    [JsonConverter(typeof(CancelOrderConverter))]
    public class CancelOrderRequest
    {
        public CancelOrderRequest(long id)
        {
            BfxValidations.ValidateInput(id, nameof(id), 0);

            Id = id;
        }

        /// <summary>
        /// Unique id (not cid)
        /// </summary>
        public long Id { get; }
    }
}
