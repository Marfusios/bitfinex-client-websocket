using Bitfinex.Client.Websocket.Logging;
using Bitfinex.Client.Websocket.Messages;
using Bitfinex.Client.Websocket.Responses;
using Bitfinex.Client.Websocket.Responses.Books;
using Bitfinex.Client.Websocket.Responses.Candles;
using Bitfinex.Client.Websocket.Responses.Configurations;
using Bitfinex.Client.Websocket.Responses.Fundings;
using Bitfinex.Client.Websocket.Responses.Status;
using Bitfinex.Client.Websocket.Responses.Tickers;
using Bitfinex.Client.Websocket.Responses.Trades;

namespace Bitfinex.Client.Websocket.Client
{
    internal class BitfinexPublicHandler
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger(); 

        private readonly BitfinexClientStreams _streams;
        private readonly BitfinexChannelList _channelIdToHandler;

        public BitfinexPublicHandler(BitfinexClientStreams streams, BitfinexChannelList channelIdToHandler)
        {
            _streams = streams;
            _channelIdToHandler = channelIdToHandler;
        }

        public void OnObjectMessage(string msg)
        {
            var parsed = BitfinexSerialization.Deserialize<MessageBase>(msg);

            switch (parsed.Event)
            {
                case MessageType.Pong:
                    PongResponse.Handle(msg, _streams.PongSubject);
                    break;
                case MessageType.Error:
                    ErrorResponse.Handle(msg, _streams.ErrorSubject);
                    break;
                case MessageType.Info:
                    InfoResponse.Handle(msg, _streams.InfoSubject);
                    break;
                case MessageType.Auth:
                    AuthenticationResponse.Handle(msg, _streams.AuthenticationSubject);
                    break;
                case MessageType.Conf:
                    ConfigurationResponse.Handle(msg, _streams.ConfigurationSubject);
                    break;
                case MessageType.Subscribed:
                    OnSubscription(BitfinexSerialization.Deserialize<SubscribedResponse>(msg));
                    break;
                case MessageType.Unsubscribed:
                    UnsubscribedResponse.Handle(msg, _streams.UnsubscriptionSubject);
                    break;
                //default:
                //    Log.Warning($"Missing handler for public stream, data: '{msg}'");
                //    break;
            }
        }

        private void OnSubscription(SubscribedResponse response)
        {
            _streams.SubscriptionSubject.OnNext(response);

            var channelId = response.ChanId;

            // ********************
            // ADD HANDLERS BELOW
            // ********************

            switch (response.Channel)
            {
                case "ticker":
                    _channelIdToHandler[channelId] = (data, config) => 
                        Ticker.Handle(data, response, config, _streams.TickerSubject);
                    break;
                case "trades":
                    //if pair is null means that is funding
                    if (response.Pair == null)
                    {
                        _channelIdToHandler[channelId] = (data, config) => 
                            Funding.Handle(data, response, config, _streams.FundingsSubject);
                    }
                    else
                    {
                        _channelIdToHandler[channelId] = (data, config) => 
                            Trade.Handle(data, response, config, _streams.TradesSubject, _streams.TradesSnapshotSubject);
                    }
                    break;
                case "candles":
                    _channelIdToHandler[channelId] = (data, config) => 
                        Candles.Handle(data, response, _streams.CandlesSubject);
                    break;
                case "book":
                    _channelIdToHandler[channelId] = (data, config) => 
                        Book.Handle(data, response, config, _streams.BookSubject, _streams.BookSnapshotSubject, _streams.BookChecksumSubject);
                    break;
                case "status":
                    if (response.Key.StartsWith("deriv"))
                    {
                        _channelIdToHandler[channelId] = (data, config) =>
                            DerivativePairStatus.Handle(data, response, _streams.DerivativePairSubject);
                    }

                    if (response.Key.StartsWith("liq"))
                    {
                        _channelIdToHandler[channelId] = (data, config) =>
                            LiquidationFeedStatus.Handle(data, response, _streams.LiquidationFeedSubject);
                    }

                    break;
                //default:
                //    Log.Warning($"Missing subscription handler '{response.Channel}'");
                //    break;
            }
        }

    }
}
