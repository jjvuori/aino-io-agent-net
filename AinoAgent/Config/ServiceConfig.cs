using System;

namespace Aino.Agents.Core.Config
{
    /// <summary>
    /// Class for holding message sending related configuration.
    /// </summary>
    class ServiceConfig
    {
        private bool enabled = false;
        private bool gzipEnabled = false;
        private string addressUri;
        private string addressApiKey;
        private int sendInterval;
        private int sendSizeThreshold;
        private string proxyHost = null;
        private int proxyPort = 0;


        /// <summary>
        /// Checks if the agent is enabled.
        /// </summary>
        /// <returns>Whether agent is enabled or not</returns>
        public bool IsEnabled()
        {
            return enabled;
        }

        public void SetEnabled(bool enabled)
        {
            bool apikeyblank = String.IsNullOrEmpty(this.addressApiKey);
            bool addressuriblank = String.IsNullOrEmpty(this.addressUri);

            if (enabled && (apikeyblank || addressuriblank))
            {
                throw new AgentCoreException("Cannot set logger to enabled, because address uri or apikey is missing.");
            }
            this.enabled = enabled;
        }

        /// <summary>
        /// Gets the address of aino.io API.
        /// </summary>
        /// <returns>aino.io API address</returns>
        public string GetAddressUri()
        {
            return addressUri;
        }

        /// <summary>
        /// Sets the address of aino.io API.
        /// </summary>
        /// <param name="addressUri">addressUri aino.io API address</param>
        public void SetAddressUri(string addressUri)
        {
            this.addressUri = addressUri;
        }

        /// <summary>
        /// Gets the API key for aino.io API.
        /// </summary>
        /// <returns>API key</returns>
        public string GetAddressApiKey()
        {
            return addressApiKey;
        }

        /// <summary>
        /// Sets the API key for aino.io API.
        /// </summary>
        /// <param name="addressApiKey"></param>
        public void SetAddressApiKey(string addressApiKey)
        {
            this.addressApiKey = addressApiKey;
        }

        /// <summary>
        /// Gets the send interval of messages.
        /// </summary>
        /// <returns>Send interval in milliseconds</returns>
        public int GetSendInterval()
        {
            return sendInterval;
        }

        /// <summary>
        /// Sets the send interval of messages.
        /// </summary>
        /// <param name="sendInterval">Send interval in milliseconds</param>
        public void SetSendInterval(int sendInterval)
        {
            this.sendInterval = sendInterval;
        }

        /// <summary>
        /// Gets the size threshold of message queue.
        /// </summary>
        /// <returns>Size threshold</returns>
        public int GetSendSizeThreshold()
        {
            return sendSizeThreshold;
        }

        /// <summary>
        /// Sets the size threshold of message queue.
        /// </summary>
        /// <param name="sendSizeThreshold">Size threshold</param>
        public void SetSendSizeThreshold(int sendSizeThreshold)
        {
            this.sendSizeThreshold = sendSizeThreshold;
        }

        /// <summary>
        /// Checks if gzipping is enabled.
        /// </summary>
        /// <returns>Whether gzipping is enabled</returns>
        public bool IsGzipEnabled()
        {
            return gzipEnabled;
        }

        /// <summary>
        /// Sets gzipping to enabled or disabled.
        /// </summary>
        /// <param name="gzipEnabled">True to enable</param>
        public void SetGzipEnabled(bool gzipEnabled)
        {
            this.gzipEnabled = gzipEnabled;
        }

        /// <summary>
        /// Gets the address of an HTTP(S) proxy.
        /// </summary>
        /// <returns>HTTP(S) proxy address</returns>
        public string GetProxyHost()
        {
            return proxyHost;
        }

        /// <summary>
        /// Sets the address of an HTTP(S) proxy.
        /// </summary>
        /// <param name="proxyHost">HTTP(S) proxy address</param>
        public void SetProxyHost(string proxyHost)
        {
            this.proxyHost = proxyHost;
        }

        /// <summary>
        /// Gets the port of an HTTP(S) proxy.
        /// </summary>
        /// <returns>HTTP(S) proxy port</returns>
        public int GetProxyPort()
        {
            return proxyPort;
        }

        /// <summary>
        /// Sets the port of an HTTP(S) proxy.
        /// </summary>
        /// <param name="proxyPort">HTTP(S) proxy port</param>
        public void SetProxyPort(int proxyPort)
        {
            this.proxyPort = proxyPort;
        }

        /// <summary>
        /// Checks if an HTTP(S) proxy is defined
        /// </summary>
        /// <returns>Whether HTTP(S) proxy is defined</returns>
        public bool IsProxyDefined()
        {
            return !string.IsNullOrEmpty(GetProxyHost());
        }
    }
}
