using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Aino;
using NUnit.Framework;

namespace AinoTests
{
    [TestFixture]
    class QueueTest
    {

        [Test]
        public void TestToJson()
        {
            using (var stream = new MemoryStream())
            {
                var queue = new MessageQueue();

                var msg = new AinoMessage()
                {
                    To = "too",
                    From = "froom"
                };
                queue.Enqueue(msg);

                queue.ToJson(stream);

                // ToJson clears the queue.
                Assert.AreEqual(0, queue.Count);

                var jsonString = Encoding.UTF8.GetString(stream.ToArray());

                Assert.IsTrue(jsonString.Contains("\"transactions\":"));
                Assert.IsTrue(jsonString.Contains("\"to\":\"too\""));
                Assert.IsTrue(jsonString.Contains("\"status\":\"unknown\""));
                Assert.IsTrue(jsonString.Contains("\"timestamp\":"));
            }
        }

        [Test]
        public void TestConcurrentAdding()
        {
            MessageQueue queue = new MessageQueue();
            var index = 0;
            var totalCount = 50000;

            Action<MessageQueue> addStuff = (sharedQueue) =>
            {
                var msg = new AinoMessage();
                msg.From = "from";
                msg.To = "to";
                var i = Interlocked.Increment(ref index);
                msg.FlowId = string.Format("{0}", i - 1);
                sharedQueue.Enqueue(msg);
            };

            Action[] actions = new Action[totalCount];

            for (var i = 0; i < totalCount; i++)
            {
                actions[i] = () => { addStuff(queue); };
            }

            Parallel.Invoke(actions);

            Assert.AreEqual(totalCount, queue.Count);

            var allMessages = queue.DequeueAll();
            Assert.AreEqual(totalCount, allMessages.Count);

            var neededFlowIds = new List<int>();
            for (var i = 0; i < totalCount; i++)
            {
                neededFlowIds.Add(i);
            }

            foreach (var message in allMessages)
            {
                if (neededFlowIds.Contains(int.Parse(message.FlowId)))
                {
                    neededFlowIds.Remove(int.Parse(message.FlowId));
                }
                else
                {
                    throw new Exception("Flow id was not found: " + message.FlowId + " and had " + neededFlowIds.Count + " items in neededFlowIds");
                }
            }

            Assert.AreEqual(0, neededFlowIds.Count, "Still had " + neededFlowIds.Count + " items in neededFlowIds");
        }

    }
}
