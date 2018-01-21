using Bitfinex.Client.Websocket.Validations;

namespace Bitfinex.Client.Websocket.Requests
{
    public class TickerSubscribeRequest : SubscribeRequestBase
    {
        public TickerSubscribeRequest(string pair)
        {
            BfxValidations.ValidateInput(pair, nameof(pair));

            Symbol = FormatPairToSymbol(pair);
        }

        public override string Channel => "ticker";
        public string Symbol { get; }
    }
}
