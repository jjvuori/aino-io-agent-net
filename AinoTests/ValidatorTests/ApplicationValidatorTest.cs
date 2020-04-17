using Aino.Agents.Core;
using Aino.Agents.Core.Config;
using NUnit.Framework;

namespace AinoTests.ValidatorTests
{
    [TestFixture]
    class ApplicationValidatorTest
    {
        //Todo: This seems to sometime fail unit tests if all are run at once - Some teardown needed?
        //Or is this some threading error?
        private Agent agent;

        private string validconfigfile = "Aino.config.validConfig.xml";

        [SetUp]
        public void SetUp()
        {
            agent = Agent.GetFactory()
                    .SetConfigurationBuilder(new ClassPathResourceConfigBuilder(validconfigfile))
                    .Build();
        }

        [Test]
        public void TestDoesNotThrowWithFromApplication()
        {
            Transaction tle = new Transaction(agent.GetAgentConfig());
            tle.SetFromKey("app01");
            tle.SetOperationKey("create");
            tle.SetToKey("esb");

            agent.AddTransaction(tle);
        }

        [Test]
        public void TestThrowsWithInvalidFromApplication()
        {
            Assert.Throws<AgentCoreException>(delegate
            {
                Transaction tle = new Transaction(agent.GetAgentConfig());
                tle.SetFromKey("app09");
                tle.SetToKey("esb");

                agent.AddTransaction(tle);
            });
        }

        [Test]
        public void TestDoesNotThrowWithToApplication()
        {
            Transaction tle = new Transaction(agent.GetAgentConfig());
            tle.SetToKey("esb");
            tle.SetFromKey("app01");
            tle.SetOperationKey("delete");

            agent.AddTransaction(tle);
        }

        [Test]
        public void TestThrowsWithInvalidToApplication()
        {
            Assert.Throws<AgentCoreException>(delegate
            {
                Transaction tle = new Transaction(agent.GetAgentConfig());
                tle.SetToKey("esb01");
                tle.SetFromKey("app01");
                tle.SetOperationKey("update");

                agent.AddTransaction(tle);
            });
        }
    }
}
