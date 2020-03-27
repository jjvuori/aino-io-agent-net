using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Aino.Agents.Core
{
    /**
     * Helper class for {@link Sender}.
     * Checks HTTP status codes etc.
     * Does logging.
     */
    class SenderStatus
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int MAX_RETRIES = 4;
        public bool retryLastSend = false;
        public int retryCount = 0;

        private bool lastSendSuccessful;
        private int lastResponseStatus;
        private string lastResponse;

        private void CreateLogMessagesForStatus()
        {
            if (lastSendSuccessful && log.IsDebugEnabled)
            {
                log.Debug(BuildStatusLogMessage());
            }
            if (!lastSendSuccessful && log.IsErrorEnabled)
            {
                log.Error(BuildStatusLogMessage());
            }
        }

        private string BuildStatusLogMessage()
        {
            if (lastSendSuccessful)
            {
                return new StringBuilder("Succeeded in sending LogEntries. HTTP status code: ").Append(this.lastResponseStatus).ToString();
            }

            if (retryCount == MAX_RETRIES)
            {
                return new StringBuilder("Failed to send LogEntries after ").Append(retryCount + 1).Append(" tries. Discarding the entries.").ToString();
            }

            if (-1 == this.lastResponseStatus)
            {
                return "Failed to send LogEntries. Connection timed out.";
            }

            StringBuilder sb = new StringBuilder("Failed to send LogEntries.");
            sb.Append(" HTTP status code: ").Append(lastResponseStatus);
            sb.Append(" Response body: ").Append(lastResponse);
            return sb.ToString();
        }

        private void HandleResponseStatus()
        {
            if (IsHttpStatus2xx())
            {
                lastSendSuccessful = true;
                return;
            }

            if (IsHttpStatus4xx())
            {
                // A malformed request. It will not get correct by retrying.
                lastSendSuccessful = false;
                retryLastSend = false;
                return;
            }

            lastSendSuccessful = false;
            retryLastSend = true;
        }

        private bool IsHttpStatus2xx()
        {
            return IsInInclusiveRange(200, 299, lastResponseStatus);
        }

        private bool IsHttpStatus4xx()
        {
            return IsInInclusiveRange(400, 499, lastResponseStatus);
        }

        private bool IsInInclusiveRange(int lowerBound, int upperBound, int value)
        {
            if (value < lowerBound)
            {
                return false;
            }
            if (value > upperBound)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Sets the initial state.
        /// </summary>
        public void InitialStatus()
        {
            lastSendSuccessful = false;
            lastResponseStatus = 0;
            lastResponse = null;
        }

        /// <summary>
        /// Updates state from response.
        /// </summary>
        /// <param name="response">Response to update the state from</param>
        public void ResponseStatus(IApiResponse response)
        {
            lastResponseStatus = response.GetStatus();
            lastResponse = response.GetPayload();
            HandleResponseStatus();
        }

        /// <summary>
        /// Sets status to error.
        /// Enables retries.
        /// </summary>
        public void ExceptionStatus()
        {
            lastSendSuccessful = false;
            lastResponseStatus = -1;
            retryLastSend = true;
        }

        /// <summary>
        /// Logs sending status.
        /// If last send was successful or max retries tried, reset some internal variables.
        /// </summary>
        public void ContinuationStatus()
        {
            CreateLogMessagesForStatus();
            if (lastSendSuccessful || retryCount == MAX_RETRIES)
            {
                retryLastSend = false;
                retryCount = 0;
            }
        }
    }
}
