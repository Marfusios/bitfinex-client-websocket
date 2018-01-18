using System.Collections.Generic;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Responses.Candles
{
    [JsonConverter(typeof(CandlesConverter))]
    public class Candles
    {
        public List<Candle> CandleList { get; set; }
        public BitfinexTimeFrame TimeFrame { get; set; }
        public string Pair { get; set; }

        public Candles()
        {
            CandleList = new List<Candle>();
        }
    }
}