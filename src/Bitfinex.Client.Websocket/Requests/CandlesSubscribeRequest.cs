using Bitfinex.Client.Websocket.Utils;
using Bitfinex.Client.Websocket.Validations;

namespace Bitfinex.Client.Websocket.Requests
{
    public class CandlesSubscribeRequest : SubscribeRequestBase
    {
        public CandlesSubscribeRequest(string pair, BitfinexTimeFrame bitfinexTimeFrame)
        {
            BfxValidations.ValidateInput(pair, nameof(pair));

            Key = $"trade:{TimeFrameToKeyCommand(bitfinexTimeFrame)}:{FormatPairToSymbol(pair)}";
        }

        public override string Channel => "candles";
        public string Key { get; set; }

        private string TimeFrameToKeyCommand(BitfinexTimeFrame bitfinexTimeFrame)
        {
            return bitfinexTimeFrame.GetStringValue();
        }
    }
}
