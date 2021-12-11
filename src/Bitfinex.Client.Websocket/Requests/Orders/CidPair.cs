using System;
using Bitfinex.Client.Websocket.Validations;

namespace Bitfinex.Client.Websocket.Requests.Orders;

/// <summary>
/// Unique client order identification - id + date
/// </summary>
public class CidPair
{
    /// <summary>
    /// Create a new unique order identification
    /// </summary>
    public CidPair(long cid, DateTime cidDate)
    {
        if (cid < 0) throw new ArgumentException(BfxValidations.LessThanZero, nameof(cid));
        if (cidDate == default) throw new ArgumentException(BfxValidations.Default, nameof(cidDate));

        Cid = cid;
        CidDate = cidDate;
    }

    /// <summary>
    /// Client id, unique per day
    /// </summary>
    public long Cid { get; }

    /// <summary>
    /// Order creation date to make Cid unique
    /// </summary>
    public DateTime CidDate { get; }
}