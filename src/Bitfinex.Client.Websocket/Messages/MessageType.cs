namespace Bitfinex.Client.Websocket.Messages;

public enum MessageType
{
    Undefined,
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