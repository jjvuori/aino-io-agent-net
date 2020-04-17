using Aino.Agents.Core;
using Aino.Agents.Core.Config;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AinoTests.ValidatorTests
{
    //Todo: This seems to sometime fail unit tests if all are run at once - Some teardown needed?
    /*Message: 
        System.Threading.SynchronizationLockException : Object synchronization method was called from an unsynchronized block of code.
    Stack Trace: 
        Monitor.ObjPulse(Object obj)
        Sender.LogDataAdded(Int32 newSize) line 58
        TransactionDataBuffer.AddTransaction(TransactionSerializable entry) line 44
        Agent.AddTransaction(Transaction entry) line 158
        OperationValidatorTest.TestDoesNotThrowWithMissingOperation() line 34
    */

    [TestFixture]
    class OperationValidatorTest
    {
        private Agent agent;
        private string validconfigfile = "Aino.config.validConfig.xml";

        [SetUp]
        public void SetUp()
        {
            agent = Agent
                    .GetFactory()
                    .SetConfigurationBuilder(new ClassPathResourceConfigBuilder(validconfigfile))
                    .Build();
        }

        [Test]
        public void TestDoesNotThrowWithMissingOperation()
        {
            Transaction tle = new Transaction(agent.GetAgentConfig());
            tle.SetFromKey("esb");
            tle.SetToKey("app01");

            agent.AddTransaction(tle);
        }

        [Test]
        public void TestDoesNotThrowWithValidOperation()
        {
            Transaction tle = new Transaction(agent.GetAgentConfig());
            tle.SetFromKey("esb");
            tle.SetToKey("app01");
            tle.SetOperationKey("create");

            agent.AddTransaction(tle);
        }

        [Test]
        public void TestThrowsWithInvalidOperation()
        {
            Assert.Throws<AgentCoreException>(delegate
            {
                Transaction tle = new Transaction(agent.GetAgentConfig());
                tle.SetFromKey("esb");
                tle.SetToKey("app01");
                tle.SetOperationKey("updaeet");

                agent.AddTransaction(tle);
            });
        }
    }
}
