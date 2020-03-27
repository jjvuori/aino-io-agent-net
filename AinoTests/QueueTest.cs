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

        
         

        /* SenderTest
         
                public static void initConfigs() throws FileNotFoundException {
                    validConfig = new FileConfigBuilder(new File("src/test/resources/validConfig.xml")).build();
                    validConfig.setSendInterval(100);
                }

                @Before
                public void initMocks() {
                    MockitoAnnotations.initMocks(this);
                }

                @Test
                public void testRemainingDataIsSentOnShutdown() throws IOException, InterruptedException {
                    final int trxCount = 10;
                    TransactionDataBuffer dataBuffer = initDataBuffer(trxCount);
                    when(apiClient.send(any(byte[].class))).thenReturn(apiResponse);
                    Sender sender = new Sender(validConfig, dataBuffer, apiClient);
                    new Thread(sender).start();
                    sender.stop();
                    Thread.sleep(1500l);
                    verify(apiClient, times(trxCount)).send(any(byte[].class));
                }

                private TransactionDataBuffer initDataBuffer(int trxCount) {
                    TransactionDataBuffer dataBuffer = new TransactionDataBuffer(1);
                    for (int i = 0; i < trxCount; i++) {

                        Transaction transaction = new Transaction(validConfig);
                        transaction.setFromKey("app01");
                        transaction.setToKey("app02");
                        transaction.setStatus("success");
                        dataBuffer.addTransaction(TransactionSerializable.from(transaction));
                    }

                    return dataBuffer;
                }
         */

        /* TransactionDataBufferTest
         
                @Test
                public void testBufferReturnsSingleTransactionWhenSizeThresholdIsZero() throws IOException {
                    TransactionDataBuffer buffer = new TransactionDataBuffer(0);
                    assertSingleTransactionsAreReturned(buffer);
                }

                @Test
                public void testBufferReturnsSingleTransactionWhenSizeThresholdIsOne() throws IOException {
                    TransactionDataBuffer buffer = new TransactionDataBuffer(1);
                    assertSingleTransactionsAreReturned(buffer);
                }

                @Test
                public void testBufferReturnsAllTransactionsWhenSizeThresholdGreaterThanOne() throws IOException {
                    TransactionDataBuffer buffer = new TransactionDataBuffer(2);
                    buffer.addTransaction(transactionWrapper());
                    buffer.addTransaction(transactionWrapper());
                    assertNotNull("Should have received data to send", buffer.getDataToSend());
                    assertEquals("No more transactions should exist", 0, buffer.getSize());
                }

                private void assertSingleTransactionsAreReturned(TransactionDataBuffer buffer) throws IOException {
                    buffer.addTransaction(transactionWrapper());
                    buffer.addTransaction(transactionWrapper());
                    assertNotNull("Should have received data to send", buffer.getDataToSend());
                    assertEquals("Should have returned one transaction", 1, buffer.getSize());
                    assertNotNull("Should have received data to send", buffer.getDataToSend());
                    assertEquals("No more transactions should exist", 0, buffer.getSize());
                }

                private TransactionSerializable transactionWrapper() {
                    return TransactionSerializable.from(simpleTransation());
                }

                private Transaction simpleTransation() {
                    Transaction transaction = new Transaction(config);
                    transaction.setFromKey("app1");
                    transaction.setToKey("app2");
                    transaction.setStatus("success");

                    return transaction;
                } 
         
         */

        /* AgentFactoryTest
         
                @Test
                public void testGetFactoryWorksWithValidConfigurationFile() throws Exception {
                    Agent agent = null;

                    try {
                        Agent.LoggerFactory factory = Agent.getFactory();
                        factory.setConfigurationBuilder(new FileConfigBuilder(new File(fileName)));
                        agent = factory.build();
                    } catch(Exception e){
                        //it happens
                    }

                    assertEquals(expectedValidity, null != agent);
                }
        */

        // Config Tests
        // KeyNameListConfigTest
        // ApplicationValidatorTest
        // IdTypeValidatorTest
        // OperationValidatorTest
        // AgentIntegrationTest


    }
}
