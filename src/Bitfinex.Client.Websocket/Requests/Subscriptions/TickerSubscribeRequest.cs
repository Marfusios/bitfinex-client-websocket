using Bitfinex.Client.Websocket.Utils;
using Bitfinex.Client.Websocket.Validations;

namespace Bitfinex.Client.Websocket.Requests.Subscriptions
{
    public class TickerSubscribeRequest : SubscribeRequestBase
    {
        public TickerSubscribeRequest(string pair)
        {
            BfxValidations.ValidateInput(pair, nameof(pair));

            Symbol = BitfinexSymbolUtils.FormatPairToTradingSymbol(pair);
        }

        public override string Channel => "ticker";
        public string Symbol { get; }
    }
}
