using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Responses.Candles
{
    [JsonConverter(typeof(CandlesConverter))]
    public class Candles
    {
        public List<Candle> CandleList { get; set; }

        public Candles()
        {
            CandleList = new List<Candle>();
        }
    }
}