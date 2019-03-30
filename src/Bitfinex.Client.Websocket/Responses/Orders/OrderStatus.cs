namespace Bitfinex.Client.Websocket.Responses.Orders
{
    public enum OrderStatus
    {
        Undefined,
        Active,
        Executed,
        PartiallyFilled,
        Canceled,
        PostOnlyCanceled,
        RsnPosReduceFlip,
        RsnPosReduceIncr
    }
}
