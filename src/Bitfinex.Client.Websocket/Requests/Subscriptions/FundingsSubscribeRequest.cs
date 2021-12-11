using System;
using Bitfinex.Client.Websocket.Utils;
using Bitfinex.Client.Websocket.Validations;

namespace Bitfinex.Client.Websocket.Requests.Subscriptions;

public class FundingsSubscribeRequest : SubscribeRequestBase
{
    public FundingsSubscribeRequest(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol)) throw new ArgumentException(BfxValidations.NullOrWhitespace, nameof(symbol));

        Symbol = BitfinexSymbolUtils.FormatSymbolToFunding(symbol);
    }

    public override string Channel => "trades";
    public string Symbol { get; }
}