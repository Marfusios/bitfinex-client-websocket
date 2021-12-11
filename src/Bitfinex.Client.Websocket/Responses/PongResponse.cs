using System;
using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Client;
using Bitfinex.Client.Websocket.Json;
using Bitfinex.Client.Websocket.Messages;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Responses;

public class PongResponse : MessageBase
{
    public int Cid { get; set; }

    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime Ts { get; set; }


    internal static void Handle(string msg, Subject<PongResponse> subject)
    {
        var pong = BitfinexSerialization.Deserialize<PongResponse>(msg);
        subject.OnNext(pong);
    }
}