using Aino.Agents.Core;
using Aino.Agents.Core.Config;
using NUnit.Framework;
using System.IO;

namespace AinoTests
{
    [TestFixture]
    class AgentTest
    {
        [Test]
        public void TestGetFactoryLoggerIsDisabledIfNotConfigured()
        {
            Assert.Throws<InvalidAgentConfigException>(delegate { Agent.GetFactory().Build(); });
        }

        [Test]
        public void TestGetFactoryThrowsWithNonExistentConfigurationFile()
        {
            Assert.Throws<FileNotFoundException>(delegate {
                string relativeconfigfilepath = @"AinoTests\config\nonexistingconfigfille.xml";
                string absoluteconfigfilepath = new FileInfo(relativeconfigfilepath).FullName;
                FileConfigBuilder fileconfigbuilder = new FileConfigBuilder(File.Open(absoluteconfigfilepath, FileMode.Open));
                Agent.GetFactory().SetConfigurationBuilder(fileconfigbuilder).Build();
            });
        }

        [Test]
        public void TestShutdownAgent()
        {
            string relativeconfigfilepath = @"AinoTests\config\validConfigWithProxy.xml";
            string absoluteconfigfilepath = new FileInfo(relativeconfigfilepath).FullName;

            FileConfigBuilder fileconfigbuilder = new FileConfigBuilder(File.Open(absoluteconfigfilepath, FileMode.Open));
            Agent agent = Agent.GetFactory().SetConfigurationBuilder(fileconfigbuilder).Build();

            agent.IncreaseThreads();
            agent.IncreaseThreads();
            Assert.AreEqual(agent.GetSenderThreadCount(), 3, "thread count");
            agent.Shutdown();
            Assert.AreEqual(agent.GetSenderThreadCount(), 0, "thread count");
        }
    }
}
