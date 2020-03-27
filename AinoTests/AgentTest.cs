using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aino.Agents.Core;
using NUnit.Framework;

namespace AinoTests
{
    [TestFixture]
    class AgentTest
    {
        [Test]
        public void TestGetFactoryLoggerIsDisabledIfNotConfigured()
        {
            Agent.GetFactory().Build();
    }


}
}

/*
//AgentTest 
@Test(expected = InvalidAgentConfigException.class)
            public void testGetFactoryLoggerIsDisabledIfNotConfigured() throws Exception
{
    Agent.getFactory().build();
}

@Test(expected = FileNotFoundException.class)
            public void testGetFactoryThrowsWithNonExistentConfigurationFile() throws Exception
{
    Agent.getFactory()
                        .setConfigurationBuilder(new FileConfigBuilder(new File("path/to/config/file")))
                        .build();
            }

            @Test
            public void testShutdownAgent() throws Exception
{
    Agent agent = Agent.getFactory().setConfigurationBuilder(new FileConfigBuilder(new File("src/test/resources/validConfig.xml"))).build();

agent.increaseThreads();
                agent.increaseThreads();
                assertEquals("thread count", agent.getSenderThreadCount(), 3);

agent.shutdown();
                assertEquals("thread count", agent.getSenderThreadCount(), 0);
            }
            
*/
