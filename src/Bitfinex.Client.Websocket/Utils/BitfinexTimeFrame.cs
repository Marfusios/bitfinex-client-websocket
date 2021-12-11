namespace Bitfinex.Client.Websocket.Utils;

public enum BitfinexTimeFrame
{
    [StringValue("1m")]
    OneMinute = 10,

    [StringValue("5m")]
    FiveMinutes = 20,

    [StringValue("15m")]
    FifteenMinutes = 30,

    [StringValue("30m")]
    ThirtyMinutes = 40,

    [StringValue("1h")]
    OneHour = 50,

    [StringValue("3h")]
    ThreeHours = 60,

    [StringValue("4h")]
    FourHours = 61,

    [StringValue("6h")]
    SixHours = 70,

    [StringValue("12h")]
    TwelveHours = 80,

    [StringValue("1D")]
    OneDay = 90,

    [StringValue("7D")]
    OneWeek = 100,

    [StringValue("14D")]
    TwoWeeks = 110,

    [StringValue("1M")]
    OneMonth = 120,
}