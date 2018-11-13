namespace Bitfinex.Client.Websocket.Communicator
{
    public enum ReconnectionType
    {
        /// <summary>
        /// Type used for initial connection to websocket stream
        /// </summary>
        Initial,

        /// <summary>
        /// Type used when connection to websocket stream was lost in meantime
        /// </summary>
        Lost,

        /// <summary>
        /// Type used when connection to websocket stream was lost by not receiving any message in given timerange
        /// </summary>
        NoMessageReceived, 

        /// <summary>
        /// Type used after unsuccessful previous reconnection to websocket stream
        /// </summary>
        Error
    }
}
