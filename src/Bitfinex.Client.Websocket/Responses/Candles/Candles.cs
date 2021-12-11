using System.Reactive.Subjects;
using Bitfinex.Client.Websocket.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Responses.Candles;

[JsonConverter(typeof(CandlesConverter))]
public class Candles : ResponseBase
{
    public Candle[] CandleList { get; set; }
    public BitfinexTimeFrame TimeFrame { get; set; }
    public string Pair { get; set; }

    internal static void Handle(JToken token, SubscribedResponse subscription, Subject<Candles> subject)
    {
        var data = token[1];

        if (data.Type != JTokenType.Array)
        {
            // probably heartbeat, ignore
            return;
        }

        var candles = data.ToObject<Candles>();

        candles.TimeFrame = new BitfinexTimeFrame().GetFieldByStringValue(subscription.Key.Split(':')[1]);
        candles.Pair = subscription.Key.Split(':')[2].Remove(0, 1);
        candles.ChanId = subscription.ChanId;
        subject.OnNext(candles);
    }
}