namespace Bitfinex.Client.Websocket.Utils
{
    public enum BitfinexFrequency
    {
        [StringValue("F0")]
        Realtime = 10,

        [StringValue("F1")]
        TwoSecDelay = 20
    }
}
