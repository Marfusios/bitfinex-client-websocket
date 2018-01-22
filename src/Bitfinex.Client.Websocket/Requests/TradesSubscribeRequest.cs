using Bitfinex.Client.Websocket.Validations;

namespace Bitfinex.Client.Websocket.Requests
{
    public class TradesSubscribeRequest : SubscribeRequestBase
    {
        public TradesSubscribeRequest(string pair)
        {
            BfxValidations.ValidateInput(pair, nameof(pair));

            Symbol = FormatPairToSymbol(pair);
        }

        public override string Channel => "trades";
        public string Symbol { get; }
    }
}
