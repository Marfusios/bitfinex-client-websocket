using System;
using System.Collections.Generic;
using Bitfinex.Client.Websocket.Messages;
using Bitfinex.Client.Websocket.Responses;
using Bitfinex.Client.Websocket.Responses.Books;
using Bitfinex.Client.Websocket.Responses.Candles;
using Bitfinex.Client.Websocket.Responses.Fundings;
using Bitfinex.Client.Websocket.Responses.Tickers;
using Bitfinex.Client.Websocket.Responses.Trades;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Bitfinex.Client.Websocket.Client
{
    internal class BitfinexPublicHandler
    {
        private readonly BitfinexClientStreams _streams;
        private readonly Dictionary<int, Action<JToken>> _channelIdToHandler;

        public BitfinexPublicHandler(BitfinexClientStreams streams, Dictionary<int, Action<JToken>> channelIdToHandler)
        {
            _streams = streams;
            _channelIdToHandler = channelIdToHandler;
        }

        public void OnObjectMessage(string msg)
        {
            var parsed = BitfinexSerialization.Deserialize<MessageBase>(msg);

            switch (parsed.Event)
            {
                case MessageType.Error:
                    ErrorResponse.Handle(msg, _streams.ErrorSubject);
                    break;
                case MessageType.Info:
                    InfoResponse.Handle(msg, _streams.InfoSubject);
                    break;
                case MessageType.Auth:
                    AuthenticationResponse.Handle(msg, _streams.AuthenticationSubject);
                    break;
                case MessageType.Pong:
                    PongResponse.Handle(msg, _streams.PongSubject);
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
                    _channelIdToHandler[channelId] = data => Ticker.Handle(data, response, _streams.TickerSubject);
                    break;
                case "trades":
                    //if pair is null means that is funding
                    if (response.Pair == null)
                        _channelIdToHandler[channelId] = data => Funding.Handle(data, response, _streams.FundingsSubject);
                    else
                        _channelIdToHandler[channelId] = data => Trade.Handle(data, response, _streams.TradesSubject);
                    break;
                case "candles":
                    _channelIdToHandler[channelId] = data => Candles.Handle(data, response, _streams.CandlesSubject);
                    break;
                case "book":
                    _channelIdToHandler[channelId] = data => Book.Handle(data, response, _streams.BookSubject, _streams.BookSnapshotSubject);
                    break;
                //default:
                //    Log.Warning($"Missing subscription handler '{response.Channel}'");
                //    break;
            }
        }

    }
}
