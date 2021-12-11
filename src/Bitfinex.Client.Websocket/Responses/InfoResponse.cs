using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Client;
using Bitfinex.Client.Websocket.Messages;

namespace Bitfinex.Client.Websocket.Responses;

public class InfoResponse : MessageBase
{
    public string Version { get; set; }
    public string Code { get; set; }
    public string Msg { get; set; }

    internal static void Handle(string msg, Subject<InfoResponse> subject)
    {
        var info = BitfinexSerialization.Deserialize<InfoResponse>(msg);
        subject.OnNext(info);
    }
}