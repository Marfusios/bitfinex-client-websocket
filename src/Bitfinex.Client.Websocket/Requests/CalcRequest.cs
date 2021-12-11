using Bitfinex.Client.Websocket.Requests.Converters;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests;

/// <summary>
/// Calculation request.
/// Calculations are on demand, so no more streaming of unnecessary data.
/// Websocket server allows up to 30 calculations per batch.
/// If the client sends too many concurrent requests (or tries to spam) requests,
/// it will receive an error and potentially a disconnection.
/// The Websocket server performs a maximum of 8 calculations per second per client.
/// </summary>
[JsonConverter(typeof(CalcConverter))]
public class CalcRequest
{
    /// <summary>
    /// requests e.g. (
    ///   margin_base, margin_sym_SYMBOL,
    ///   funding_sym_SYMBOL, position_SYMBOL,
    ///   wallet_WALLET-TYPE_CURRENCY, balance
    /// )
    /// </summary>
    public string[] Requests { get; set; }

    /// <inheritdoc />
    public CalcRequest(string[] requests)
    {
        Requests = requests;
    }
}