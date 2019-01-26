using System;

namespace Bitfinex.Client.Websocket.Utils
{
    /// <summary>
    /// Utils for UNIX time
    /// </summary>
    public static class BitfinexTime
    {
        /// <summary>
        /// UNIX base
        /// </summary>
        public static readonly DateTime UnixBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Now in UNIX time
        /// </summary>
        /// <returns></returns>
        public static long NowMs()
        {
            var subtracted = DateTime.UtcNow.Subtract(UnixBase);
            return (long)subtracted.TotalMilliseconds;
        }

        /// <summary>
        /// Convert UNIX time to DateTime
        /// </summary>
        /// <param name="timeInMs"></param>
        /// <returns></returns>
        public static DateTime ConvertToTime(long timeInMs)
        {
            return UnixBase.AddMilliseconds(timeInMs);
        }

        /// <summary>
        /// Convert UNIX time to DateTime or null
        /// </summary>
        /// <param name="timeInMs"></param>
        /// <returns></returns>
        public static DateTime? ConvertToTime(long? timeInMs)
        {
            if (!timeInMs.HasValue)
                return null;
            return UnixBase.AddMilliseconds(timeInMs.Value);
        }
    }
}
