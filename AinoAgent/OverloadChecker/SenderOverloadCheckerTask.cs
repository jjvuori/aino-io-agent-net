using Aino.Agents.Core.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


/// <summary>
/// Timer task to check if send queue is well above size threshold.
/// If buffer size is more than 30% bigger than the size threshold, increase sending threads.
/// This is necessary when system is generating lots of messages and connection to aino.io is slow:
/// send buffer size increases and transactions get bigger... which leads bigger and bigger HTTP post requests.
/// </summary>
namespace Aino.Agents.Core.OverloadChecker
{
    class SenderOverloadCheckerTask
    {
        IThreadAmountObserver observer;
        TransactionDataBuffer buffer;
        AgentConfig config;

        public SenderOverloadCheckerTask(IThreadAmountObserver obs, TransactionDataBuffer buffer, AgentConfig config)
        {

            this.buffer = buffer;
            this.config = config;
            this.observer = obs;
        }

        public void Run()
        {
            if (buffer.IsEmpty() || config.GetSizeThreshold() <= 1)
                return;

            if (config.GetSizeThreshold() < buffer.GetSize() * 1.3)
            {
                observer.IncreaseThreads();
            }
        }
    }
}
