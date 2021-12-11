using System;
using Bitfinex.Client.Websocket.Utils;
using Bitfinex.Client.Websocket.Validations;

namespace Bitfinex.Client.Websocket.Requests.Subscriptions;

/// <summary>
/// Subscribe for L3 order book data (no grouping, every single order)
/// </summary>
public class RawBookSubscribeRequest : SubscribeRequestBase
{
    /// <summary>
    /// Subscribe for L3 order book data (no grouping, every single order)
    /// </summary>
    /// <param name="pair">Target symbol/pair (BTCUSD, ETHBTC, etc.)</param>
    /// <param name="length">Number of orders returned by API ("1", "25", "100") [default="25"]</param>
    public RawBookSubscribeRequest(
        string pair, 
        string length = null)
    {
        if (string.IsNullOrWhiteSpace(pair)) throw new ArgumentException(BfxValidations.NullOrWhitespace, nameof(pair));

        Symbol = BitfinexSymbolUtils.FormatPairToSymbol(pair);
        Prec = "R0";
        Len = string.IsNullOrWhiteSpace(length) ? "25" : length;
    }

    /// <summary>
    /// Websocket channel API
    /// </summary>
    public override string Channel => "book";

    /// <summary>
    /// Target symbol/pair
    /// </summary>
    public string Symbol { get; }

    /// <summary>
    /// Level of price aggregation, R0 - raw, realtime
    /// </summary>
    public string Prec { get; }

    /// <summary>
    /// Number of orders returned by API ("1", "25", "100") [default="25"]
    /// </summary>
    public string Len { get; set; }
}