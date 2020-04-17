using Aino.Agents.Core;
using Aino.Agents.Core.Config;
using NUnit.Framework;
using System;
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
            string relativeconfigfilepath = @"AinoTests\config";
            string absoluteconfigfiledir = new FileInfo(relativeconfigfilepath).FullName + Path.DirectorySeparatorChar;
            string absoluteconfigfilepath = new FileInfo(absoluteconfigfiledir + "nonexistingconfigfille.xml").FullName;
            Assert.IsTrue(Directory.Exists(absoluteconfigfiledir), "Directory doesn't exist");
            Assert.Throws<FileNotFoundException>(delegate
            {
                FileConfigBuilder fileconfigbuilder = new FileConfigBuilder(File.Open(absoluteconfigfilepath, FileMode.Open));
                Agent.GetFactory().SetConfigurationBuilder(fileconfigbuilder).Build();
            });
        }

        [Test]
        public void TestShutdownAgent()
        {
            string relativeconfigfilepath = @"AinoTests\config\";
            string absoluteconfigfiledir = new FileInfo(relativeconfigfilepath).FullName + Path.DirectorySeparatorChar;
            string absoluteconfigfilepath = new FileInfo(absoluteconfigfiledir + "validConfigWithProxy.xml").FullName;
            FileConfigBuilder fileconfigbuilder = new FileConfigBuilder(File.Open(absoluteconfigfilepath, FileMode.Open));
            Agent agent = Agent.GetFactory().SetConfigurationBuilder(fileconfigbuilder).Build();

            agent.IncreaseThreads();
            agent.IncreaseThreads();
            Assert.AreEqual(agent.GetSenderThreadCount(), 3, "thread count");
            agent.Shutdown();
            Assert.AreEqual(agent.GetSenderThreadCount(), 0, "thread count");
        }


        [TestCase("validConfig.xml", true)]
        //[TestCase("validConfigWithIntervalAndSize.xml", true)] // Todo: Check GZipEnabled of this file.
        [TestCase("validConfigWithProxy.xml", true)]
        [TestCase("invalidConfig.xml", false)]
        public void TestGetFactoryWorksWithValidConfigurationFile(string file, bool expectedresult)
        {
            Agent agent = null;
            try
            {
                Agent.LoggerFactory factory = Agent.GetFactory();

                string relativeconfigfilepath = @"AinoTests\config\";
                string absoluteconfigfiledir = new FileInfo(relativeconfigfilepath).FullName + Path.DirectorySeparatorChar;
                string absoluteconfigfilepath = new FileInfo(absoluteconfigfiledir + file).FullName;
                FileConfigBuilder fileconfigbuilder = new FileConfigBuilder(File.Open(absoluteconfigfilepath, FileMode.Open));


                factory.SetConfigurationBuilder(fileconfigbuilder);
                agent = factory.Build();
            }
            catch (Exception)
            {
                //it happens
            }

            Assert.AreEqual(expectedresult, agent != null);
        }
    }
}
