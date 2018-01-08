using Bitfinex.Client.Websocket.Messages;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests
{
    public abstract class RequestBase : MessageBase
    {
        public override MessageType Event
        {
            get { return EventType; }
            set { }
        }

        [JsonIgnore]
        public abstract MessageType EventType { get; }
    }
}