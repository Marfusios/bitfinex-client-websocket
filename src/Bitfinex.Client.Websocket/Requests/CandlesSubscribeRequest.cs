using System;

namespace Bitfinex.Client.Websocket.Requests
{
    public class CandlesSubscribeRequest : SubscribeRequestBase
    {
        public override string Channel => "candles";
        public string Key { get; set; }


        public CandlesSubscribeRequest(string pair,TimeFrame timeFrame)
        {
            Key = $"trade:{TimeFrameToKeyCommand(timeFrame)}:{FormatPairToSymbol(pair)}";
        }

        private string TimeFrameToKeyCommand(TimeFrame timeFrame)
        {
            switch (timeFrame)
            {
                case TimeFrame.OneMinute:
                    return "1m";
                case TimeFrame.FiveMinute:
                    return "5m";
                case TimeFrame.OneDay:
                    return "1D";
                    default:throw new NotImplementedException();
            }
        }

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

    public enum TimeFrame
    {
        OneMinute = 10,
        FiveMinute = 20,
        OneDay = 30,
    }
}
