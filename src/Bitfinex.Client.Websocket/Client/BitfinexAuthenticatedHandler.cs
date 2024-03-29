﻿using System.Linq;
using Bitfinex.Client.Websocket.Responses.Balance;
using Bitfinex.Client.Websocket.Responses.Configurations;
using Bitfinex.Client.Websocket.Responses.Notifications;
using Bitfinex.Client.Websocket.Responses.Margin;
using Bitfinex.Client.Websocket.Responses.Orders;
using Bitfinex.Client.Websocket.Responses.Positions;
using Bitfinex.Client.Websocket.Responses.Trades;
using Bitfinex.Client.Websocket.Responses.TradesPrivate;
using Bitfinex.Client.Websocket.Responses.Wallets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Client.Websocket.Client
{
    internal class BitfinexAuthenticatedHandler
    {
        private readonly ILogger _logger;
        private readonly BitfinexClientStreams _streams;

        public BitfinexAuthenticatedHandler(BitfinexClientStreams streams, BitfinexChannelList channelIdToHandler, ILogger logger)
        {
            _streams = streams;
            _logger = logger;

            channelIdToHandler[0] = HandleAccountInfo;
        }

        private string L(string msg)
        {
            return $"[BFX AUTHENTICATED HANDLER] {msg}";
        }

        internal void HandleAccountInfo(JToken token, ConfigurationState config)
        {
            var itemsCount = token?.Count();
            if (token == null || itemsCount < 2)
            {
                _logger.LogWarning(L("Invalid message format, too low items"));
                return;
            }

            var secondItem = token[1];
            if (secondItem?.Type != JTokenType.String)
            {
                _logger.LogWarning(L("Invalid message format, second param is not string"));
                return;
            }
            var msgType = (string?)secondItem;
            if (msgType == "hb")
            {
                // heartbeat, ignore
                return;
            }

            if (itemsCount < 3)
            {
                _logger.LogWarning(L("Invalid message format, too low items"));
                return;
            }

            // ********************
            // ADD HANDLERS BELOW
            // ********************

            switch (msgType)
            {
                case "ws":
                    Wallet.Handle(token, _streams.WalletSubject, _streams.WalletsSubject);
                    break;
                case "wu":
                    Wallet.Handle(token, _streams.WalletSubject);
                    break;
                case "os":
                    Order.Handle(token, _streams.OrdersSubject);
                    break;
                case "on":
                    Order.Handle(token, _streams.OrderCreatedSubject);
                    break;
                case "ou":
                    Order.Handle(token, _streams.OrderUpdatedSubject);
                    break;
                case "oc":
                    Order.Handle(token, _streams.OrderCanceledSubject);
                    break;
                case "te":
                    PrivateTrade.Handle(token, config, _streams.PrivateTradeSubject, TradeType.Executed);
                    break;
                case "tu":
                    PrivateTrade.Handle(token, config, _streams.PrivateTradeSubject, TradeType.UpdateExecution);
                    break;
                case "ps":
                    Position.Handle(token, config, _streams.PositionsSubject);
                    break;
                case "pn":
                    Position.Handle(token, config, _streams.PositionCreatedSubject);
                    break;
                case "pu":
                    Position.Handle(token, config, _streams.PositionUpdatedSubject);
                    break;
                case "pc":
                    Position.Handle(token, config, _streams.PositionCanceledSubject);
                    break;
                case "n":
                    Notification.Handle(token, _streams.NotificationSubject);
                    break;
                case "bu":
                    BalanceInfo.Handle(token, _streams.BalanceInfoSubject);
                    break;
                case "miu":
                    MarginInfo.Handle(token, _streams.MarginInfoSubject);
                    break;
                    //default:
                    //    Log.Warning($"Missing private handler for '{msgType}'. Data: {token}");
                    //    break;
            }
        }
    }
}
