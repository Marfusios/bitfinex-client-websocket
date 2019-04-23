namespace Bitfinex.Client.Websocket.Responses.Orders
{
    /// <summary>
    /// Type of the order
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Undefined,

        /// <summary>
        /// Margin limit (maker) order.
        /// A limit order is one of the most basic order types.
        /// It allows the trader to specify a price and amount they would like to buy or sell.
        /// </summary>
        Limit,

        /// <summary>
        /// Margin market (taker) order.
        /// A market order is an order type that executes immediately against the best price available.
        /// As long as there are willing sellers and buyers, market orders are filled.
        /// </summary>
        Market,

        /// <summary>
        /// Margin stop loss order.
        /// A stop order is used to trigger a market sell when the market drops to your trigger (stop) price,
        /// or used to trigger a market buy if the market rises to your trigger (stop) price.
        /// </summary>
        Stop,

        /// <summary>
        /// Margin trailing stop loss order.
        /// A trailing stop order provides flexibility over a stop order by executing
        /// once the market goes against you by a defined price, called the price distance.
        /// When margin trading, a trailing stop sell order can be used to protect profit.
        /// </summary>
        TrailingStop,

        /// <summary>
        /// Spot limit (maker) order.
        /// A limit order is one of the most basic order types.
        /// It allows the trader to specify a price and amount they would like to buy or sell.
        /// </summary>
        ExchangeLimit,

        /// <summary>
        /// Spot market (taker) order.
        /// A market order is an order type that executes immediately against the best price available.
        /// As long as there are willing sellers and buyers, market orders are filled.
        /// </summary>
        ExchangeMarket,

        /// <summary>
        /// Spot stop loss order.
        /// A stop order is used to trigger a market sell when the market drops to your trigger (stop) price,
        /// or used to trigger a market buy if the market rises to your trigger (stop) price.
        /// </summary>
        ExchangeStop,

        /// <summary>
        /// Spot trailing stop loss order.
        /// A trailing stop order provides flexibility over a stop order by executing
        /// once the market goes against you by a defined price, called the price distance.
        /// </summary>
        ExchangeTrailingStop,

        /// <summary>
        /// Margin fill or kill order.
        /// A "fill or kill" order is a limit order that must be filled immediately in its entirety or it is canceled (killed).
        /// The purpose of a fill or kill order is to ensure that a position is entered instantly and at a specific price.
        /// </summary>
        Fok,

        /// <summary>
        /// Spot fill or kill order.
        /// A "fill or kill" order is a limit order that must be filled immediately in its entirety or it is canceled (killed).
        /// The purpose of a fill or kill order is to ensure that a position is entered instantly and at a specific price.
        /// </summary>
        ExchangeFok,

        /// <summary>
        /// A margin stop-limit order executes as a limit order within a specific price range (buy or sell limit price or better)
        /// and not as a market order.
        /// With a stop-limit, the trader sets a stop price at which the order is triggered
        /// and a limit price at which the order may be filled.
        /// </summary>
        StopLimit,

        /// <summary>
        /// A spot stop-limit order executes as a limit order within a specific price range (buy or sell limit price or better)
        /// and not as a market order.
        /// With a stop-limit, the trader sets a stop price at which the order is triggered
        /// and a limit price at which the order may be filled.
        /// </summary>
        ExchangeStopLimit
    }
}
