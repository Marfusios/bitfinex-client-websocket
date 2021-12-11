namespace Bitfinex.Client.Websocket.Responses;

/// <summary>
/// Side of the funding
/// </summary>
public enum FundingSide
{
    Borrower = -1,
    Both = 0,
    Lender = 1
}