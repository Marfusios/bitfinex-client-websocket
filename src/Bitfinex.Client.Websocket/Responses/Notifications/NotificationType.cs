namespace Bitfinex.Client.Websocket.Responses.Notifications
{
    /// <summary>
    /// Notification type
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// Unknown type
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// On a new order request
        /// </summary>
        OnReq,

        /// <summary>
        /// On cancel order request
        /// </summary>
        OcReq,

        /// <summary>
        /// ???
        /// </summary>
        Uca,

        /// <summary>
        /// On a new funding offer request
        /// </summary>
        FonReq,

        /// <summary>
        /// On cancel funding offer request
        /// </summary>
        FocReq,

        /// <summary>
        /// On update order request
        /// </summary>
        OuReq,

        /// <summary>
        /// On wallet transfer (between exchange --> margin --> funding)
        /// </summary>
        WalletTransfer
    }
}