using System;
using Bitfinex.Client.Websocket.Requests.Converters;
using Bitfinex.Client.Websocket.Validations;
using Newtonsoft.Json;

namespace Bitfinex.Client.Websocket.Requests.Orders;

/// <summary>
/// Request to update active order. 
/// </summary>
[JsonConverter(typeof(UpdateOrderConverter))]
public class UpdateOrderRequest
{
    /// <summary>
    /// Create update request for unique Bitfinex order id
    /// </summary>
    /// <param name="id"></param>
    public UpdateOrderRequest(long id)
    {
        if (id < 0) throw new ArgumentException(BfxValidations.LessThanZero, nameof(id));

        Id = id;
    }

    /// <summary>
    /// Create update request for unique pair client id + datetime (when client id was generated)
    /// </summary>
    public UpdateOrderRequest(CidPair cidPair)
    {
        if (cidPair == null) throw new ArgumentNullException(nameof(cidPair));

        CidPair = cidPair;
    }


    /// <summary>
    /// Unique Bitfinex id (must be set)
    /// </summary>
    public long? Id { get; }

    /// <summary>
    /// Unique client order identification
    /// </summary>
    public CidPair CidPair { get; }

    /// <summary>
    /// Group id for the order
    /// </summary>
    public long? Gid { get; set; }

    /// <summary>
    /// Price
    /// </summary>
    public double? Price { get; set; }

    /// <summary>
    /// Positive for buy, Negative for sell
    /// </summary>
    public double? Amount { get; set; }

    /// <summary>
    /// Change of amount
    /// </summary>
    public double? Delta { get; set; }

    /// <summary>
    /// Auxiliary Limit price (for STOP LIMIT)
    /// </summary>
    public double? PriceAuxLimit { get; set; }

    /// <summary>
    /// The trailing price
    /// </summary>
    public double? PriceTrailing { get; set; }

    /// <summary>
    /// Additional order configuration, see OrderFlag enum. 
    /// You may sum flag values to pass multiple flags. For example passing 4160 (64 + 4096) means hidden post only.
    /// Use C# [Flags] to do that: Flags = OrderFlag.Hidden | OrderFlag.PostOnly
    /// </summary>
    public OrderFlag? Flags { get; set; }

    /// <summary>
    /// Time-In-Force: datetime for automatic order cancellation (ie. 2020-01-01 10:45:23) )
    /// </summary>
    public DateTime? TimeInForce { get; set; }
}