namespace Bitfinex.Client.Websocket.Messages
{
    public enum MessageType
    {
        Info,
        Auth,
        Error,
        Ping,
        Pong,
        Conf,
        Subscribe,
        Subscribed,
        Unsubscribe,
        Unsubscribed
    }
}
