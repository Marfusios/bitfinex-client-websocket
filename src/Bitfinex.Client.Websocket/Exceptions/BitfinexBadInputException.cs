using System;

namespace Bitfinex.Client.Websocket.Exceptions;

public class BitfinexBadInputException : BitfinexException
{
    public BitfinexBadInputException()
    {
    }

    public BitfinexBadInputException(string message) : base(message)
    {
    }

    public BitfinexBadInputException(string message, Exception innerException) : base(message, innerException)
    {
    }
}