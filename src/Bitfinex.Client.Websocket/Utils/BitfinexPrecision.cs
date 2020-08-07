namespace Bitfinex.Client.Websocket.Utils
{
    /// <summary>
    /// Price precision returned by order book API.
    /// Significant Digits Calculations: https://support.bitfinex.com/hc/en-us/articles/115000371105-How-is-precision-calculated-using-Significant-Digits
    /// </summary>
    public enum BitfinexPrecision
    {
        /// <summary>
        /// Price with 5 significant figures
        /// </summary>
        [StringValue("P0")]
        P0 = 10,

        /// <summary>
        /// Price with 4 significant figures
        /// </summary>
        [StringValue("P1")]
        P1 = 20,

        /// <summary>
        /// Price with 3 significant figures
        /// </summary>
        [StringValue("P2")]
        P2 = 30,

        /// <summary>
        /// Price with 2 significant figures
        /// </summary>
        [StringValue("P3")]
        P3 = 40,

        /// <summary>
        /// Price with 1 significant figure
        /// </summary>
        [StringValue("P4")]
        P4 = 50
    }
}
