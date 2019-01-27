namespace Bitfinex.Client.Websocket.Responses.Fundings
{
    /// <summary>
    /// Type of the funding
    /// </summary>
    public enum FundingType
    {
        /// <summary>
        /// Initial information (faster)
        /// </summary>
        Executed,

        /// <summary>
        /// Extended information (slower)
        /// </summary>
        UpdateExecution
    }
}