using System;
using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Client;
using Bitfinex.Client.Websocket.Messages;

namespace Bitfinex.Client.Websocket.Responses;

public class ErrorResponse : MessageBase
{
    public string Code { get; set; }
    public string Msg { get; set; }

    internal static void Handle(string msg, Action<string> logError, Subject<ErrorResponse> subject)
    {
        var error = BitfinexSerialization.Deserialize<ErrorResponse>(msg);
        logError($"Error received - message: {error.Msg}, code: {error.Code}");
        subject.OnNext(error);
    }
}