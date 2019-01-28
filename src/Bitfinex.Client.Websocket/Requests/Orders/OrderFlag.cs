using System;

namespace Bitfinex.Client.Websocket.Requests.Orders
{
    /// <summary>
    /// Additional configuration for the order
    /// </summary>
    [Flags]
    public enum OrderFlag
    {
        /// <summary>
        /// None
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The hidden order option ensures an order does not appear in the order book; thus does not influence other market participants.
        /// </summary>
        Hidden = 64,

        /// <summary>
        /// Close position if position present.
        /// </summary>
        Close = 512,

        /// <summary>
        /// Ensures that the executed order does not flip the opened position.
        /// </summary>
        ReduceOnly = 1024,

        /// <summary>
        /// The post-only limit order option ensures the limit order will be added to the order book and not match with a pre-existing order.
        /// </summary>
        PostOnly = 4096,

        /// <summary>
        /// The one cancels other order option allows you to place a pair of orders stipulating that if one order is executed fully or partially,
        /// then the other is automatically canceled.
        /// </summary>
        OCO = 16384
    }
}
