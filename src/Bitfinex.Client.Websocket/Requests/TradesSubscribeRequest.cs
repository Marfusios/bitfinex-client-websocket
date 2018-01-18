using Bitfinex.Client.Websocket.Validations;

namespace Bitfinex.Client.Websocket.Requests
{
    public class TradesSubscribeRequest: SubscribeRequestBase
    {
        public TradesSubscribeRequest(string pair)
        {
            BfxValidations.ValidateInput(pair, nameof(pair));

            Symbol = FormatPairToSymbol(pair);
        }

        public override string Channel => "trades";
        public string Symbol { get; }


        private string FormatPairToSymbol(string pair)
        {
            var formatted = pair
                .Trim()
                .Replace("/", string.Empty)
                .Replace("\\", string.Empty)
                .ToUpper();
            return $"t{formatted}";
        }
    }
}
