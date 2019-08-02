namespace Bitfinex.Client.Websocket.Requests.Subscriptions
{
    public class StatusSubscribeRequest : SubscribeRequestBase
    {
        /// <summary>
        /// Subscribe to and receive different types of platform information - currently supports derivatives pair status and liquidation feed.
        /// </summary>
        /// <param name="key">"deriv:{symbol}" or "liq:global"</param>
        public StatusSubscribeRequest(string key)
        {
            Key = key;
        }

        public override string Channel => "status";
        public string Key { get; set; }
    }
}