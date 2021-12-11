using System;
using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Client;
using Bitfinex.Client.Websocket.Messages;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Responses;

public class AuthenticationResponse : MessageBase
{
    public string Status { get; set; }
    public int ChanId { get; set; }
    public int UserId { get; set; }
    public string Code { get; set; }
    public object Caps { get; set; }

    /// <summary>
    /// Returns true if authentication succeed
    /// </summary>
    [JsonIgnore] 
    public bool IsAuthenticated => Status == "OK";
        
    internal static void Handle(string msg, Action<string> logWarning, Subject<AuthenticationResponse> subject)
    {
        var response = BitfinexSerialization.Deserialize<AuthenticationResponse>(msg);
        ;           if (!response.IsAuthenticated)
            logWarning("Authentication failed. Code: " + response.Code);
        subject.OnNext(response);
    }
}