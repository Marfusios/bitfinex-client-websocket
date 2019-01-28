using System.Linq;
using Bitfinex.Client.Websocket.Responses.Configurations;
using Bitfinex.Client.Websocket.Responses.Orders;
using Bitfinex.Client.Websocket.Responses.Positions;
using Bitfinex.Client.Websocket.Responses.Trades;
using Bitfinex.Client.Websocket.Responses.TradesPrivate;
using Bitfinex.Client.Websocket.Responses.Wallets;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Bitfinex.Client.Websocket.Client
{
    internal class BitfinexAuthenticatedHandler
    {
        private readonly BitfinexClientStreams _streams;

        public BitfinexAuthenticatedHandler(BitfinexClientStreams streams, BitfinexChannelList channelIdToHandler)
        {
            _streams = streams;

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
                //default:
                //    Log.Warning($"Missing private handler for '{msgType}'. Data: {token}");
                //    break;
            }
        }
    }
}
