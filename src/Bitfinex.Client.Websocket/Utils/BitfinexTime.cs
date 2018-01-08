using System;

namespace Bitfinex.Client.Websocket.Utils
{
    public static class BitfinexTime
    {

        public static long NowMs()
        {
            var substracted = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1));
            return (long)substracted.TotalMilliseconds;
        }

    }
}
