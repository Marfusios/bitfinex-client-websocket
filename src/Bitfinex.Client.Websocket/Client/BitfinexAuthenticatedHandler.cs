using System;
using System.Collections.Generic;
using System.Linq;
using Bitfinex.Client.Websocket.Responses.Orders;
using Bitfinex.Client.Websocket.Responses.Wallets;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Bitfinex.Client.Websocket.Client
{
    internal class BitfinexAuthenticatedHandler
    {
        private readonly BitfinexClientStreams _streams;

        public BitfinexAuthenticatedHandler(BitfinexClientStreams streams, Dictionary<int, Action<JToken>> channelIdToHandler)
        {
            _streams = streams;

            channelIdToHandler[0] = HandleAccountInfo;
        }

        private string L(string msg)
        {
            return $"[BFX AUTHENTICATED HANDLER] {msg}";
        }

        internal void HandleAccountInfo(JToken token)
        {
            var itemsCount = token?.Count();
            if (token == null || itemsCount < 2)
            {
                Log.Warning($"Invalid message format, too low items");
                return;
            }

            var secondItem = token[1];
            if (secondItem.Type != JTokenType.String)
            {
                Log.Warning(L("Invalid message format, second param is not string"));
                return;
            }
            var msgType = (string)secondItem;
            if (msgType == "hb")
            {
                // heartbeat, ignore
                return;
            }

            if (itemsCount < 3)
            {
                Log.Warning(L("Invalid message format, too low items"));
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
            }
        }
    }
}
