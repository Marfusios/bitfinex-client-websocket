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
        WalletTransfer,

        /// <summary>
        /// On closing a position
        /// </summary>
        PosClose,

        /// <summary>
        /// When a new deposit is detected
        /// </summary>
        DepositNew,

        /// <summary>
        /// When a deposit has been credited
        /// </summary>
        DepositComplete,

        /// <summary>
        /// On multi new order request
        /// </summary>
        OnMultiReq,

        /// <summary>
        /// On multi cancel order request
        /// </summary>
        OcMultiReq
    }
}