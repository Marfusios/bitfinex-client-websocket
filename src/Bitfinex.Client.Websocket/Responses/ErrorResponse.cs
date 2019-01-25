using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Client;
using Bitfinex.Client.Websocket.Messages;
using Serilog;

using static Bitfinex.Client.Websocket.Client.BitfinexLogger;

namespace Bitfinex.Client.Websocket.Responses
{
    public class ErrorResponse : MessageBase
    {
        public string Code { get; set; }
        public string Msg { get; set; }


        internal static void Handle(string msg, Subject<ErrorResponse> subject)
        {
            var error = BitfinexSerialization.Deserialize<ErrorResponse>(msg);
            Log.Error(L($"Error received - message: {error.Msg}, code: {error.Code}"));
            subject.OnNext(error);
        }
    }
}
