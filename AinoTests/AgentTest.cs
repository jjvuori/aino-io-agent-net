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
        private TestContext testContextInstance;
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        //string relativeconfigfilepath = @"AinoTests"+ Path.DirectorySeparatorChar + "config";
        
        [Test]
        public void TestGetFactoryLoggerIsDisabledIfNotConfigured()
        {
            Assert.Throws<InvalidAgentConfigException>(delegate { Agent.GetFactory().Build(); });
        }

        [Test]
        public void TestGetFactoryThrowsWithNonExistentConfigurationFile()
        {
            string relativeconfigfilepath = TestContext.CurrentContext.WorkDirectory+Path.DirectorySeparatorChar+@"config";
            string absoluteconfigfiledir = new FileInfo(relativeconfigfilepath).FullName + Path.DirectorySeparatorChar;
            string absoluteconfigfilepath = new FileInfo(absoluteconfigfiledir + "nonexistingconfigfille.xml").FullName;
            Assert.IsTrue(Directory.Exists(absoluteconfigfiledir), "Directory doesn't exist"+absoluteconfigfilepath);
            Assert.Throws<FileNotFoundException>(delegate
            {
                FileConfigBuilder fileconfigbuilder = new FileConfigBuilder(File.Open(absoluteconfigfilepath, FileMode.Open));
                Agent.GetFactory().SetConfigurationBuilder(fileconfigbuilder).Build();
            });
        }

        [Test]
        public void TestShutdownAgent()
        {
            string relativeconfigfilepath = TestContext.CurrentContext.WorkDirectory + Path.DirectorySeparatorChar + @"config";
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
            string relativeconfigfilepath = TestContext.CurrentContext.WorkDirectory + Path.DirectorySeparatorChar + @"config";
            Agent agent = null;
            try
            {
                Agent.LoggerFactory factory = Agent.GetFactory();
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
