using Aino.Agents.Core;
using Aino.Agents.Core.Config;
using NUnit.Framework;

namespace AinoTests
{
    [TestFixture]
    class TransactionDataBufferTest
    {
        private AgentConfig config;

        [OneTimeSetUp]
        public void SetUp()
        {
            config = new AgentConfig();
            config.GetApplications().AddEntry("app1", "Application 1");
            config.GetApplications().AddEntry("app2", "Application 2");
        }

        [Test]
        public void TestBufferReturnsSingleTransactionWhenSizeThresholdIsZero()
        {
            TransactionDataBuffer buffer = new TransactionDataBuffer(0);
            AssertSingleTransactionsAreReturned(buffer);
        }

        [Test]
        public void TestBufferReturnsSingleTransactionWhenSizeThresholdIsOne()
        {
            TransactionDataBuffer buffer = new TransactionDataBuffer(1);
            AssertSingleTransactionsAreReturned(buffer);
        }

        [Test]
        public void TestBufferReturnsAllTransactionsWhenSizeThresholdGreaterThanOne()
        {
            TransactionDataBuffer buffer = new TransactionDataBuffer(2);
            buffer.AddTransaction(TransactionWrapper());
            buffer.AddTransaction(TransactionWrapper());
            Assert.NotNull(buffer.GetDataToSend(), "Should have received data to send");
            Assert.AreEqual(buffer.GetSize(), 0, "No more transactions should exist");
        }

        private void AssertSingleTransactionsAreReturned(TransactionDataBuffer buffer)
        {
            buffer.AddTransaction(TransactionWrapper());
            buffer.AddTransaction(TransactionWrapper());

            Assert.NotNull(buffer.GetDataToSend(), "Should have received data to send");
            Assert.AreEqual(buffer.GetSize(), 1, "Should have returned one transaction");
            Assert.NotNull(buffer.GetDataToSend(), "Should have received data to send");
            Assert.AreEqual(buffer.GetSize(), 0, "No more transactions should exist");
        }

        private TransactionSerializable TransactionWrapper()
        {
            return TransactionSerializable.From(SimpleTransation());
        }

        private Transaction SimpleTransation()
        {
            Transaction transaction = new Transaction(config);
            transaction.SetFromKey("app1");
            transaction.SetToKey("app2");
            transaction.SetStatus("success");

            return transaction;
        }
    }   
}
