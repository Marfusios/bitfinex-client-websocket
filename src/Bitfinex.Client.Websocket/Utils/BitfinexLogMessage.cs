namespace Bitfinex.Client.Websocket.Utils;

static class BitfinexLogMessage
{
    public static string Public(string message) => Format("PUBLIC", message);
    public static string Authenticated(string message) => Format("AUTHENTICATED", message);

    static string Format(string type, string message) => $"[BFX {type} WEBSOCKET CLIENT] {message}";
}