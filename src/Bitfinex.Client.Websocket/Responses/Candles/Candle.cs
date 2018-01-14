using System;

namespace Bitfinex.Client.Websocket.Responses.Candles
{
    public class Candle
    {
        public long Mts { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Volume { get; set; }

        public DateTime TimeStamp
        {
            get
            {
                var posixTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);
                return posixTime.AddMilliseconds(Mts);
            }
        }
    }
}
