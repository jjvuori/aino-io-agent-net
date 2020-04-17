using Aino.Agents.Core;
using Aino.Agents.Core.Config;
using NUnit.Framework;
using System.Collections.Generic;

namespace AinoTests.ValidatorTests
{
    [TestFixture]
    class IdTypeValidatorTest
    {
        //Todo: This seems to sometime fail unit tests if all are run at once - Some teardown needed?
        //Or is this some threading error?

        private Agent agent;
        private Transaction tle;
        private string validconfigfile = "Aino.config.validConfig.xml";

        [SetUp]
        public void SetUp()
        {
            agent = Agent
                    .GetFactory()
                    .SetConfigurationBuilder(new ClassPathResourceConfigBuilder(validconfigfile))
                    .Build();

            tle = new Transaction(agent.GetAgentConfig());
            tle.SetFromKey("esb");
            tle.SetToKey("app01");
            tle.SetOperationKey("create");
        }

        [Test]
        public void TestDoesNotThrowWithValidIdType()
        {
            List<string> testlist = new List<string>() { "441" };
            tle.AddIdsByTypeKey("dataType01", testlist);
            agent.AddTransaction(tle);
        }

        [Test]
        public void TestThrowsWithInvalidIdType()
        {
            Assert.Throws<AgentCoreException>(delegate
            {
                List<string> testlist = new List<string>() { "441", "6666" };
                tle.AddIdsByTypeKey("invalidDataType", testlist);

                agent.AddTransaction(tle);
            });
        }

        [Test]
        public void TestDoesNotThrowWithMultipleValidIdTypes()
        {
            List<string> idList1 = tle.AddIdTypeKey("dataType01");
            List<string> idList2 = tle.AddIdTypeKey("dataType02");

            idList1.Add("111");
            idList2.Add("555");

            agent.AddTransaction(tle);
        }

        [Test]
        public void TestThrowsWithOneValidAndOneInvalidIdType()
        {
            Assert.Throws<AgentCoreException>(delegate
            {
                List<string> idListValid = tle.AddIdTypeKey("dataType01");
                List<string> idListInvalid = tle.AddIdTypeKey("Invalid");

                idListInvalid.Add("661");
                idListValid.Add("11");

                agent.AddTransaction(tle);
            });
        }
    }
}
