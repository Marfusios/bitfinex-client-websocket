using Bitfinex.Client.Websocket.Utils;
using Bitfinex.Client.Websocket.Validations;

namespace Bitfinex.Client.Websocket.Requests
{
    public class FundingsSubscribeRequest : SubscribeRequestBase
    {
        public FundingsSubscribeRequest(string symbol)
        {
            BfxValidations.ValidateInput(symbol, nameof(symbol));

            Symbol = BitfinexSymbolUtils.FormatSymbolToFunding(symbol);
        }

        public override string Channel => "trades";
        public string Symbol { get; }
    }
}
