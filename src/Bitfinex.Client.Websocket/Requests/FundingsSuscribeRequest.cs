using Bitfinex.Client.Websocket.Validations;

namespace Bitfinex.Client.Websocket.Requests
{
    public class FundingsSuscribeRequest : SubscribeRequestBase
    {
        public FundingsSuscribeRequest(string symbol)
        {
            BfxValidations.ValidateInput(symbol, nameof(symbol));

            Symbol = FormatSymbolFunding(symbol);
        }

        public override string Channel => "trades";
        public string Symbol { get; }
    }
}
