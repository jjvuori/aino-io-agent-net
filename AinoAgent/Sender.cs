using Aino.Agents.Core.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using log4net;

namespace Aino.Agents.Core
{
    class Sender : ITransactionDataObserver
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Todo: Java's Runnable is ThreadStart in C#, but it is not an interface. It is the core thread class.

        private enum Action
        {
            RETRY,
            SEND,
            NONE
        }


        private int ContinueLoopFlag = 1;
        private SenderStatus Status = new SenderStatus();

        private readonly AgentConfig agentConfig;
        private readonly TransactionDataBuffer transactionDataBuffer;
        private readonly IApiClient client;
        private string stringToSend;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">Agent configuration</param>
        /// <param name="dataBuffer">Databuffer to use</param>
        /// <param name="client">The Aino.io API client to use</param>
        public Sender(AgentConfig config, TransactionDataBuffer dataBuffer, IApiClient client)
        {
            agentConfig = config;
            this.client = client;
            transactionDataBuffer = dataBuffer;
            transactionDataBuffer.AddLogDataSizeObserver(this);
        }

        public void Stop()
        {
            Interlocked.Exchange(ref ContinueLoopFlag, 0);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void LogDataAdded(int newSize)
        {
            if (newSize >= agentConfig.GetSizeThreshold())
            {
                Monitor.Pulse(newSize); //Todo: Is newSize the right thing to be notified?
            }
        }


        public void Run()
        {
            Status.InitialStatus();

            try
            {
                while (transactionDataBuffer.ContainsData() || Interlocked.Exchange(ref ContinueLoopFlag, 1) == 1)
                {
                    switch (SenderAction())
                    {
                        case Action.RETRY:
                            {
                                Retry();
                                break;
                            }
                        case Action.SEND:
                            {
                                Send();
                                break;
                            }
                        case Action.NONE:
                            {
                                break;
                            }
                        default:
                            {
                                Sleep();
                                break;
                            }
                    }
                }
            }
            catch (ThreadInterruptedException ignored)
            {
                // Thread has been interrupted. Stop processing.
                _ = ignored.InnerException.ToString();
            }
        }

        private Action SenderAction()
        {
            if (Status.retryLastSend)
            {
                return Action.RETRY;
            }

            if (transactionDataBuffer.ContainsData())
            {
                return Action.SEND;
            }

            return Action.NONE;
        }

        private void Retry()
        {
            PerformRequest();
            Sleep();
        }

        private void Send()
        {
            SendLogData();
            Sleep();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void Sleep()
        {
            // If sleep interval is very short, this will generate a ridiculous amount of log messages. Enable only in dire need of debugging.
            // log.trace(new StringBuilder("Sleeping for a maximum of ").append(agentConfig.getSendInterval()).append(" ms."));

            lock (this)
            {
                Monitor.Wait(agentConfig.GetSendInterval());
            }
        }

        private void SendLogData()
        {
            try
            {
                stringToSend = transactionDataBuffer.GetDataToSend();
                PerformRequest();
            }
            catch (System.IO.IOException e)
            {
                log.Error("Failed to send LogEntries because the JSON serialization failed.", e);
            }
        }

        private void PerformRequest()
        {
            try
            {
                Status.retryCount++;
                log.Debug("Attempting to resend log entries (retry " + Status.retryCount + ").");

                IApiResponse response = client.Send(GetRequestContent());

                Status.ResponseStatus(response);
            }
            catch (Exception e) // Todo: ClientHandlerException in C#?
            {
                _ = e.InnerException.ToString();
                Status.ExceptionStatus();
            }
            finally
            {
                Status.ContinuationStatus();
            }
        }

        //Todo: This probably needs something else since this has now nothing to do with gzip
        private byte[] GetRequestContent()
        {
            if (!agentConfig.IsGzipEnabled())
            {
                byte[] bytes = Encoding.ASCII.GetBytes(stringToSend);
            }

            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (GZipStream compressedstream = new GZipStream(stream, CompressionMode.Compress))
                    {
                        stream.CopyTo(compressedstream);
                        return stream.ToArray();
                    }
                }
            }
            catch (IOException e)
            {
                string a = e.InnerException.ToString();
                throw new AgentCoreException("Failed to compress Aino log message using gzip.");
            }
        }
    }
}