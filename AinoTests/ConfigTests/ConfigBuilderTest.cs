using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aino.Agents.Core.Config;
using System.IO;

namespace Aino.Agents.Core.Config.UnitTests
{
    [TestFixture]
    public class ConfigBuilderTest
    {
        private string validconfigfile = "Aino.config.validConfig.xml";
        private string invalidconfigfile = "Aino.config.invalidConfig.xml";
        private string validconfigfilewithproxy = "Aino.config.validConfigWithProxy.xml";

        [Test]
        public void TestConfigBuilderDoesNotThrowWithValidConf()
        {
            AgentConfig conf = new ClassPathResourceConfigBuilder(validconfigfile).Build();
            Assert.IsNotNull(conf, "AgentConfig object should not be null");
        }

        [Test]
        public void TestConfigBuilderThrowsWithInvalidConf()
        {
            Assert.Throws<InvalidAgentConfigException>(delegate { new ClassPathResourceConfigBuilder(invalidconfigfile).Build(); });
        }

        [Test]
        public void TestFileConfigBuilderLoadsConfigWithValidConf()
        {
            // Read embedded resource config XML and check it is not null
            //File file = new File(this.GetClass().getClassLoader().getResource("validConfig.xml").getPath());

            AgentConfig conf = new ClassPathResourceConfigBuilder(validconfigfile).Build();
            Assert.IsNotNull(conf, "AgentConfig object should not be null");
        }

        [Test]
        public void TestConfigBuilderPopulatesServiceConfigs()
        {
            AgentConfig conf = new ClassPathResourceConfigBuilder(validconfigfile).Build();
            Assert.IsNotNull(conf, "AgentConfig object should not be null");

            Assert.AreEqual("http://localhost:8808/api/1.0/saveLogArray", conf.GetLogServiceUri(), "addressUri is correct");
            Assert.AreEqual("80D0710C-2EE6-481E-BA9E-9A21C2486EE7", conf.GetApiKey(), "apiKey is correct");
            Assert.AreEqual(1000, conf.GetSendInterval(), "sendInterval is correct");
            Assert.AreEqual(0, conf.GetSizeThreshold(), "sizeThreshold is correct");
        }

        [Test]
        public void TestConfigBuilderPopulatesOperationConfigs()
        {
            AgentConfig conf = new ClassPathResourceConfigBuilder(validconfigfile).Build();

            Assert.AreEqual("Create", conf.GetOperations().GetEntry("create"), "create operation exists with name 'Create'");
            Assert.AreEqual("Update", conf.GetOperations().GetEntry("update"), "update operation exists with name 'Update'");
            Assert.AreEqual("Delete", conf.GetOperations().GetEntry("delete"), "delete operation exists with name 'Delete'");
        }

        [Test]
        public void TestConfigBuilderPopulatesIdTypeConfigs()
        {
            AgentConfig conf = new ClassPathResourceConfigBuilder(validconfigfile).Build();

            Assert.AreEqual("Data Type 1", conf.GetIdTypes().GetEntry("dataType01"), "'dataType01' idType exists with name 'Data Type 1");
            Assert.AreEqual("Data Type 5", conf.GetIdTypes().GetEntry("dataType02"), "'dataType02' idType exists with name 'Data Type 5");
        }

        [Test]
        public void TestConfigBuilderPopulatesApplicationConfigs()
        {
            AgentConfig conf = new ClassPathResourceConfigBuilder(validconfigfile).Build();

            Assert.AreEqual("ESB", conf.GetApplications().GetEntry("esb"), "application with name 'ESB' exists with key 'esb'");
            Assert.AreEqual("TestApp 1", conf.GetApplications().GetEntry("app01"), "application with name 'TestApp 1' exists with key 'app01'");
        }

        [Test]
        public void TestConfigBuilderWorksWithProxyDefinition()
        {
            AgentConfig confWithoutProxy = new ClassPathResourceConfigBuilder(validconfigfile).Build();
            Assert.AreEqual(false, confWithoutProxy.IsProxyDefined(), "proxy should not be defined");


            AgentConfig confWithProxy = new ClassPathResourceConfigBuilder(validconfigfilewithproxy).Build();
            Assert.AreEqual(true, confWithProxy.IsProxyDefined(), "proxy should be defined");
            Assert.AreEqual("127.0.0.1", confWithProxy.GetProxyHost(), "proxy host should be set");
            Assert.AreEqual(8080, confWithProxy.GetProxyPort(), "proxy port should be set");
        }
    }
}
