using System;

namespace Aino.Agents.Core.Config
{
    /// <summary>
    /// Class for holding agent configuration
    /// </summary>
    public class AgentConfig
    {
        private readonly ServiceConfig loggerService = new ServiceConfig();
        private readonly KeyNameListConfig operations = new KeyNameListConfig();
        private readonly KeyNameListConfig applications = new KeyNameListConfig();
        private readonly KeyNameListConfig idTypes = new KeyNameListConfig();
        private readonly KeyNameListConfig payloadTypes = new KeyNameListConfig();

        public enum KeyNameElementType
        {
            Operations= 0,
            Applications=1,
            Idtypes=2,
            Payloadtypes=3
        }

        /// <summary>
        /// Get the configured URL to aino.io API.
        /// </summary>
        /// <returns>Aino.io API URL</returns>
        public string GetLogServiceUri() => loggerService.GetAddressUri();

        /// <summary>
        /// Sets the URL to aino.io API.
        /// </summary>
        /// <param name="logServiceUri">URL of aino.io API</param>
        public void SetLogServiceUri(string logServiceUri) => loggerService.SetAddressUri(logServiceUri);

        /// <summary>
        /// Gets the configured API key.
        /// </summary>
        /// <returns>apikey</returns>
        public string GetApiKey() => loggerService.GetAddressApiKey();

        /// <summary>
        /// Sets the API key used for authentication to aino.io API.
        /// </summary>
        /// <param name="apiKey">apikey</param>
        public void SetApiKey(string apiKey) => loggerService.SetAddressApiKey(apiKey);

        /// <summary>
        /// Gets the interval of data sending.
        /// </summary>
        /// <returns>Interval in milliseconds</returns>
        public int GetSendInterval() => loggerService.GetSendInterval();

        /// <summary>
        /// Sets the interval for data sending.
        /// </summary>
        /// <param name="sendInterval">Interval in milliseconds</param>
        public void SetSendInterval(int sendInterval) => loggerService.SetSendInterval(sendInterval);

        /// <summary>
        /// Gets the size threshold for sending.
        /// If log message count in send buffer is over the threshold, starts sending data to aino.io API
        /// (even if <see cref="getSendInterval"/>  has not yet passed).
        /// </summary>
        /// <returns>Number of log entries</returns>
        public int GetSizeThreshold() => loggerService.GetSendSizeThreshold();

        /// <summary>
        /// Sets the size threshold of send buffer.
        /// </summary>
        /// <remarks> 
        /// See <see cref="getSizeThreshold"/> for more information on size threshold.
        /// </remarks>
        /// <param name="maxSize">Threshold</param>
        public void SetSizeThreshold(int maxSize) => loggerService.SetSendSizeThreshold(maxSize);

        /// <summary>
        /// Checks if logging to aino.io is enabled.
        /// </summary>
        /// <returns>Whether logging is enabled</returns>
        public bool IsEnabled() => loggerService.IsEnabled();

        /// <summary>
        /// Sets logging on/off.
        /// </summary>
        /// <param name="val">True to enable, false to disable</param>
        public void SetEnabled(bool val) => loggerService.SetEnabled(val);

        /// <summary>
        /// Checks if gzipping the request is enabled.
        /// </summary>
        /// <returns>Whether gzip enabled</returns>
        public bool IsGzipEnabled() => loggerService.IsGzipEnabled();

        /// <summary>
        /// Sets whether data should be gzipped when sending it to the API.
        /// </summary>
        /// <param name="val">Whether the data should be gzipped</param>
        public void SetGzipEnabled(bool val) => loggerService.SetGzipEnabled(val);

        /// <summary>
        /// Get the operations defined.
        /// </summary>
        /// <returns>KeyNameListConfig containing operations</returns>
        public KeyNameListConfig GetOperations() => operations;

        /// <summary>
        /// Get the id types defined.
        /// </summary>
        /// <returns>KeyNameListConfig containing operations</returns>
        public KeyNameListConfig GetIdTypes() => idTypes;


        /// <summary>
        /// Get the applications defined.
        /// </summary>
        /// <returns>KeyNameListConfig containing id types</returns>
        public KeyNameListConfig GetApplications() => applications;


        /// <summary>
        /// Get the payload types defined.
        /// </summary>
        /// <returns>KeyNameListConfig containing operations</returns>
        public KeyNameListConfig GetPayloadTypes() => payloadTypes;



        public KeyNameListConfig Get(KeyNameElementType type)
        {
            switch (type)
            {
                case KeyNameElementType.Operations:
                    return operations;
                case KeyNameElementType.Applications:
                    return applications;
                case KeyNameElementType.Idtypes:
                    return idTypes;
                case KeyNameElementType.Payloadtypes:
                    return payloadTypes;
                default:
                    throw new SystemException("Invalid KeyNameElement");
            }
        }

        /// <summary>
        /// Get the configured HTTP(S) proxy host.
        /// </summary>
        /// <returns>HTTP(S) proxy host address</returns>
        public string GetProxyHost() => loggerService.GetProxyHost();

        /// <summary>
        /// Sets the address to an HTTP(S) proxy.
        /// </summary>
        /// <param name="proxyHost">Address of an HTTP(S) proxy</param>
        public void SetProxyHost(string proxyHost) => loggerService.SetProxyHost(proxyHost);

        /// <summary>
        /// Get the configured HTTP(S) proxy port.
        /// </summary>
        /// <returns>HTTP(S) proxy port</returns>
        public int GetProxyPort() => loggerService.GetProxyPort();

        /// <summary>
        /// Sets the port to an HTTP(S) proxy.
        /// </summary>
        /// <param name="proxyport">Port of an HTTP(S) proxy</param>
        public void SetProxyPort(int proxyport) => loggerService.SetProxyPort(proxyport);

        /// <summary>
        /// Checks if an HTTP(S) proxy is defined
        /// </summary>
        /// <returns>Whether an HTTP(S) proxy is defined</returns>
        public bool IsProxyDefined() => loggerService.IsProxyDefined();
    }
}
