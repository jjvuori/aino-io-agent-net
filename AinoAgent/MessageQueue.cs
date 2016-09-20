using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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

        public void ToJson(Stream stream)
        {
            var firstRun = true;
            var transactionBytes = Encoding.UTF8.GetBytes("{\"transactions\"}: [");
            var commabytes = Encoding.UTF8.GetBytes(",");

            stream.Write(transactionBytes, 0, transactionBytes.Length);

            while (!IsEmpty)
            {
                AinoMessage msg;
                var success = TryDequeue(out msg);
                if(!success) continue;

                if (!firstRun)
                {
                    stream.Write(commabytes, 0, commabytes.Length);
                }

                msg.ToJson(stream);
                firstRun = false;
            }

            var endingBytes = Encoding.UTF8.GetBytes("]}");

            stream.Write(endingBytes, 0, endingBytes.Length);
        }
    }
}
