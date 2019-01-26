using System;
using System.Collections.Generic;
using Bitfinex.Client.Websocket.Responses.Configurations;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Client
{
    internal class BitfinexChannelList :  Dictionary<int, Action<JToken, ConfigurationState>>
    {
    }
}
