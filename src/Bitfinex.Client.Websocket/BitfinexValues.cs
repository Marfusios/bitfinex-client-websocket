using System;

namespace Bitfinex.Client.Websocket
{
    /// <summary>
    /// Static values
    /// </summary>
    public static class BitfinexValues
    {
        /// <summary>
        /// Url to Bitfinex websocket API (Ethfinex also works)
        /// </summary>
        public static readonly Uri ApiWebsocketUrl = new Uri("wss://api.bitfinex.com/ws/2");

        /// <summary>
        /// Url to Bitfinex websocket API (same as `ApiWebsocketUrl`)
        /// </summary>
        public static readonly Uri BitfinexWebsocketUrl = ApiWebsocketUrl;

        /// <summary>
        /// Url to Ethfinex websocket API
        /// </summary>
        public static readonly Uri EthfinexWebsocketUrl = new Uri("wss://api.ethfinex.com/ws/2");
    }
}
