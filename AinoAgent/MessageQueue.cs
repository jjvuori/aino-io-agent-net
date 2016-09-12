using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aino
{
    internal class MessageQueue : ConcurrentQueue<AinoMessage>
    {
        
        public string ToJson()
        {
            var sb = new StringBuilder("{\"transactions\": [");
            var firstRun = true;
            while (!IsEmpty)
            {
                AinoMessage msg;
                var success = TryDequeue(out msg);
                if(!success) continue;

                if (!firstRun)
                {
                    sb.Append(",");
                }

                sb.Append(msg.ToJson());
                firstRun = false;
            }

            sb.Append("]}");
            return sb.ToString();
        }
    }
}
