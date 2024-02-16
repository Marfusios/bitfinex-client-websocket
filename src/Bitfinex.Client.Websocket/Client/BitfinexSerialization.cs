using Bitfinex.Client.Websocket.Json;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Client
{
    internal static class BitfinexSerialization
    {
        public static T? Deserialize<T>(string msg) where T : class
        {
            return JsonConvert.DeserializeObject<T>(msg, BitfinexJsonSerializer.Settings);
        }
    }
}
