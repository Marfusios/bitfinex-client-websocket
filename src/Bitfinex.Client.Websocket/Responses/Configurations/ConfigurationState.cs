namespace Bitfinex.Client.Websocket.Responses.Configurations
{
    /// <summary>
    /// State of the enabled features
    /// </summary>
    public class ConfigurationState
    {
        /// <inheritdoc />
        public ConfigurationState()
        {
            
        }

        /// <summary>
        /// Select current features and create snapshot
        /// </summary>
        public ConfigurationState(bool isDecimalAsStringEnabled, bool isTimeAsStringEnabled, bool isTimestampEnabled, bool isSequencingEnabled, bool isChecksumEnabled)
        {
            IsDecimalAsStringEnabled = isDecimalAsStringEnabled;
            IsTimeAsStringEnabled = isTimeAsStringEnabled;
            IsTimestampEnabled = isTimestampEnabled;
            IsSequencingEnabled = isSequencingEnabled;
            IsChecksumEnabled = isChecksumEnabled;
        }

        public bool IsDecimalAsStringEnabled { get; }

        public bool IsTimeAsStringEnabled { get; }

        public bool IsTimestampEnabled { get; }

        public bool IsSequencingEnabled { get; }

        public bool IsChecksumEnabled { get; }
    }
}
