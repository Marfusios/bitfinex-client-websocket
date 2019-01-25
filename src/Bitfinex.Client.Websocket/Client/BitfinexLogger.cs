namespace Bitfinex.Client.Websocket.Client
{
    internal static class BitfinexLogger
    {
        public static string L(string msg)
        {
            return $"[BFX WEBSOCKET CLIENT] {msg}";
        }
    }
}
