using Bitfinex.Client.Websocket.Requests.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests
{
    [JsonConverter(typeof(CalcConverter))]
    public class CalcRequest
    {
        /// <summary>
        /// requests e.g. (margin_sym_SYMBOL, funding_sym_SYMBOL, position_SYMBOL, wallet_WALLET-TYPE_CURRENCY, balance)
        /// </summary>
        public string[] Requests { get; set; }

        public CalcRequest(string[] requests)
        {
            Requests = requests;
        }
    }
}