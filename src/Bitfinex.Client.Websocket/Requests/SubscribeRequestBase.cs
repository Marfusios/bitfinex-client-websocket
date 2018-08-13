using Bitfinex.Client.Websocket.Messages;

namespace Bitfinex.Client.Websocket.Requests
{
    public abstract class SubscribeRequestBase : RequestBase
    {
        public override MessageType EventType => MessageType.Subscribe;

        public abstract string Channel { get; }


        protected string FormatPairToSymbol(string pair)
        {
            var formatted = pair
                .Trim()
                .Replace("/", string.Empty)
                .Replace("\\", string.Empty)
                .ToUpper();
            return $"t{formatted}";
        }

        protected string FormatSymbolFunding(string symbol)
        {
            var formatted = symbol
                .Trim()
                .Replace("/", string.Empty)
                .Replace("\\", string.Empty)
                .ToUpper();
            return $"f{formatted}";
        }
    }
}
