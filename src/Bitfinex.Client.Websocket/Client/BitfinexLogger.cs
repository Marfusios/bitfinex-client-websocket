using Bitfinex.Client.Websocket.Communicator;

namespace Bitfinex.Client.Websocket.Client
{
    internal static class BitfinexLogger
    {
        public static string L(string msg, IBitfinexCommunicator communicator)
        {
            return $"[{communicator.Name ?? "BFX"} WEBSOCKET CLIENT] {msg}";
        }
    }
}
