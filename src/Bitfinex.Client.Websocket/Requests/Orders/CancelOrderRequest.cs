using System;
using Bitfinex.Client.Websocket.Requests.Converters;
using Bitfinex.Client.Websocket.Validations;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests.Orders;

/// <summary>
/// Cancel order request.
/// You can cancel the order by the Internal Order ID or using a Client Order ID (supplied by you).
/// The Client Order ID is unique per day, so you also have to provide the date of the order.
/// </summary>
[JsonConverter(typeof(CancelOrderConverter))]
public class CancelOrderRequest
{
    /// <summary>
    /// Cancel order by unique Bitfinex order id
    /// </summary>
    /// <param name="id"></param>
    public CancelOrderRequest(long id)
    {
        if (id < 0) throw new ArgumentException(BfxValidations.LessThanZero, nameof(id));

        Id = id;
    }

    /// <summary>
    /// Cancel order by combination of client id and order creation date
    /// </summary>
    public CancelOrderRequest(CidPair cidPair)
    {
        CidPair = cidPair ?? throw new ArgumentNullException(nameof(cidPair));
    }

    /// <summary>
    /// Unique Bitfinex order id (not cid)
    /// </summary>
    public long? Id { get; }

    /// <summary>
    /// Unique client order identification
    /// </summary>
    public CidPair CidPair { get; }
}