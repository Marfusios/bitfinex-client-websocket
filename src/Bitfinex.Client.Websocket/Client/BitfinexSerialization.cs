using Bitfinex.Client.Websocket.Json;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Client;

static class BitfinexSerialization
{
    public static T Deserialize<T>(string msg)
    {
        return JsonConvert.DeserializeObject<T>(msg, BitfinexJsonSerializer.Settings);
    }
}