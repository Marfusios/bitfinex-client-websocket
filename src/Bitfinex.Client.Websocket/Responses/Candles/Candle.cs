using System;

namespace Bitfinex.Client.Websocket.Responses.Candles
{
    public class Candle
    {
        public DateTime Mts { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Volume { get; set; }
    }
}
