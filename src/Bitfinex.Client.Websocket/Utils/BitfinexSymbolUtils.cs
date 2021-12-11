namespace Bitfinex.Client.Websocket.Utils;

/// <summary>
/// Utils for Bitfinex symbols, pairs formatting
/// </summary>
public static class BitfinexSymbolUtils
{
    /// <summary>
    /// Format pair into Bitfinex symbol (BTC/USD --> tBTCUSD or fBTC)
    /// </summary>
    /// <param name="pair">fUSD, tBTCUSD, etc</param>
    public static string FormatPairToSymbol(string pair)
    {
        var pairSafe = (pair ?? string.Empty);
        return pairSafe.StartsWith("f") ? 
            FormatSymbolToFunding(pairSafe) : 
            FormatPairToTradingSymbol(pairSafe);
    }

    /// <summary>
    /// Format pair into Bitfinex trading symbol (BTC/USD --> tBTCUSD)
    /// </summary>
    /// <param name="pair">BTC/USD, BTCUSD, etc</param>
    public static string FormatPairToTradingSymbol(string pair)
    {
        var pairSafe = (pair ?? string.Empty);
        return pairSafe.StartsWith("t") ? 
            pairSafe : 
            $"t{FormatPair(pairSafe)}";
    }

    /// <summary>
    /// Format symbol into Bitfinex funding symbol (BTC --> fBTC)
    /// </summary>
    /// <param name="symbol">BTC, USD, fUSD, etc</param>
    public static string FormatSymbolToFunding(string symbol)
    {
        var symbolSafe = (symbol ?? string.Empty);
        return symbolSafe.StartsWith("f") ? 
            symbolSafe : 
            $"f{FormatPair(symbolSafe)}";
    }

    /// <summary>
    /// Extract pair from symbol (tBTCUSD --> BTCUSD)
    /// </summary>
    /// <param name="symbol">tBTCUSD, fbtcusd, etc</param>
    public static string ExtractPair(string symbol)
    {
        var formatted = FormatPair(symbol);
        return !string.IsNullOrWhiteSpace(formatted) && formatted.Length > 6 ? formatted.Remove(0, 1) : string.Empty;
    }

    /// <summary>
    /// Extract funding currency from symbol (fUSD --> USD)
    /// </summary>
    /// <param name="symbol">fUSD, fusd, etc</param>
    public static string ExtractFundingCurrency(string symbol)
    {
        var formatted = FormatPair(symbol);
        return !string.IsNullOrWhiteSpace(formatted) && formatted.Length > 3 ? formatted.Remove(0, 1) : string.Empty;
    }

    /// <summary>
    /// Extract base symbol from pair (BTCUSD --> BTC)
    /// </summary>
    /// <param name="pair">BTC/USD, BTCUSD, etc</param>
    public static string ExtractBaseSymbol(string pair)
    {
        var formatted = FormatPair(pair);
        return !string.IsNullOrWhiteSpace(formatted) && formatted.Length > 5 ? formatted.Substring(0, 3) : string.Empty;
    }

    /// <summary>
    /// Extract quote symbol from pair (BTCUSD --> USD)
    /// </summary>
    /// <param name="pair">BTC/USD, BTCUSD, etc</param>
    public static string ExtractQuoteSymbol(string pair)
    {
        var formatted = FormatPair(pair);
        return !string.IsNullOrWhiteSpace(formatted) && formatted.Length > 5 ? formatted.Substring(3, 3) : string.Empty;
    }

    /// <summary>
    /// Format pair into unified style (btc/usd --> BTCUSD)
    /// </summary>
    /// <param name="pair"></param>
    /// <returns></returns>
    public static string FormatPair(string pair)
    {
        var safe = (pair ?? string.Empty);
        return safe
            .Trim()
            .Replace("/", string.Empty)
            .Replace("\\", string.Empty)
            .ToUpper();
    }
}