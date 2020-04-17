using Aino.Agents.Core;
using Aino.Agents.Core.Config;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

//Todo: This requires a lot of work. The block-commented code should be uncommented, the base there is what it should be.
namespace AinoTests.IntegrationTests
{
    class AgentIntegrationTest
    {
        private HttpClient client = new HttpClient();
        private const string API_URL = "http://localhost:8808/api/1.0";
        private const string AINO_CONFIG = "Aino.config.validConfig.xml";

        // Todo: Check GZip?
        //private const string AINO_CONFIG_WITH_LONG_INTERVAL = "Aino.config.validConfigWithIntervalAndSize.xml";
        //Replace this with that one above once that GZip is checked.
        private const string AINO_CONFIG_WITH_LONG_INTERVAL = "Aino.config.validConfigWithProxy.xml";

        private const string AINO_CONFIG_WITH_PROXY = "Aino.config.validConfigWithProxy.xml";

        /*
        private Agent ainoAgent = GetAinoLogger();
        private Agent slowAinoAgent = GetSlowAinoLogger();
        private Agent proxiedAinoAgent = GetProxiedAinoLogger();
        
        private static Agent GetAinoLogger()
        {
            Agent.LoggerFactory ainoLoggerFactory = new Agent.LoggerFactory();
            return ainoLoggerFactory.SetConfigurationBuilder(new ClassPathResourceConfigBuilder(AINO_CONFIG)).Build();
        }

        private static Agent GetSlowAinoLogger()
        {
            Agent.LoggerFactory ainoLoggerFactory = new Agent.LoggerFactory();
            return ainoLoggerFactory.SetConfigurationBuilder(new ClassPathResourceConfigBuilder(AINO_CONFIG_WITH_LONG_INTERVAL)).Build();
        }

        private static Agent GetProxiedAinoLogger()
        {
            Agent.LoggerFactory ainoLoggerFactory = new Agent.LoggerFactory();
            return ainoLoggerFactory.SetConfigurationBuilder(new ClassPathResourceConfigBuilder(AINO_CONFIG_WITH_PROXY)).Build();
        }

        private bool IsAinoMockApiUp()
        {
            try
            {
                IPAddress iPAddress = Dns.GetHostEntry("127.0.0.1").AddressList[0];
                bool reachable = IsReachable(iPAddress, 8808, 1000);
                Console.WriteLine("Aino Mock API reachable: " + reachable);
                return reachable;
            }
            catch (IOException e)
            {
                _ = e;
                Console.WriteLine("Aino Mock API is probably not running, I/O exception!");
                return false;
            }
        }
        private bool IsProxyUp()
        {
            try
            {
                IPAddress iPAddress = Dns.GetHostEntry("127.0.0.1").AddressList[0];
                bool reachable = IsReachable(iPAddress, 8080, 1000);
                Console.WriteLine("Proxy reachable: " + reachable);
                return reachable;
            }
            catch (IOException e)
            {
                _ = e;
                Console.WriteLine("Proxy is probably not running, I/O exception!");
                return false;
            }
        }

        private bool IsReachable(IPAddress host, int port, int timeout)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(host, port);
                //System.out.println("Connected to: " + host + ":" + port);
                return socket.Connected;
            }
            catch (IOException e)
            {
                _ = e;
                Console.WriteLine("Could not connect to: " + host + ":" + port);
                return false;
            }
            finally
            {
                if (socket.Connected)
                {
                    //Console.WriteLine("Closing connection to: " + host + ":" + port);
                    socket.Close();
                    //Console.WriteLine("Disconnected: " + host + ":" + port);
                }
            }
        }

        [SetUp]
        public void Setup()
        {
            string content;
            if (!IsAinoMockApiUp())
            {
                Console.WriteLine("--------------------------------------------");
                Console.WriteLine(" SKIPPING TEST - AINO MOCK API IS NOT UP!!! ");
                Console.WriteLine("--------------------------------------------");
            }
            Assert.IsTrue(IsAinoMockApiUp());

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(API_URL + "/test/clear");

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                content = reader.ReadToEnd();
            }


        }

        //[Test]
        public void AinoMockApiIsRunningTest()
        {
            string content;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(API_URL + "/ping");
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                Assert.That(response.StatusCode == HttpStatusCode.OK);
                content = reader.ReadToEnd();
                Assert.That(content == "pong");
            }
        }

        //[Test]
        public void LoggerSendsDataToMockApiTest()
        {
            AssertLoggerSendsDataToMockApi(ainoAgent);
        }

        //[Test]
        public void LoggerSendsDataToMockApiThroughProxyTest()
        {
            if (!IsProxyUp())
            {
                Console.WriteLine("------------------------------------------------------------------------");
                Console.WriteLine(" SKIPPING loggerSendsDataToMockApiThroughProxyTest - PROXY IS NOT UP!!! ");
                Console.WriteLine("------------------------------------------------------------------------");
            }

            Assert.IsTrue("Test proxy needs to be running first.", IsProxyUp());
            //TODO Test could be improved by implementing an automatic test proxy
            //instead of starting one manually. E.g. something like below
            // TestProxy proxy = new TestProxy("127.0.0.1", 8080, 8088);
            //proxy.start();
            AssertLoggerSendsDataToMockApi(proxiedAinoAgent);
            //proxy.stop();
        }

        [Test]
        public void LoggerDoesNotSendDataToMockApiThroughProxyWhenProxyIsOfflineTest()
        {
            if (IsProxyUp())
            {
                Console.WriteLine("------------------------------------------------------------------------");
                Console.WriteLine(" SKIPPING loggerSendsDataToMockApiThroughProxyTest - PROXY _IS_ UP!!!   ");
                Console.WriteLine("------------------------------------------------------------------------");
            }
            Assert.IsFalse(IsProxyUp(), "Test proxy should not be up while running this test.");
            InitializeBasicTransaction(proxiedAinoAgent);

            Thread.Sleep(2000); // :(

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(API_URL + "/test/readTransactions");
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Assert.That(response.StatusCode == HttpStatusCode.OK);
            }

            JsonNode transactions = ParseJsonFromResponseBody(get).FindPath("transactions");
            Assert.IsNotNull("JsonNode 'transactions' should not be null", transactions);
            Assert.AreEqual("There should be no transactions due to proxy failure", 0, transactions.size());
        }

        public void AssertLoggerSendsDataToMockApi(Agent agent)
        {
            InitializeBasicTransaction(agent);
            HttpStatusCode status;
            Thread.Sleep(2000); // :(
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(API_URL + "/test/readTransactions");
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                status = response.StatusCode;
                Assert.That(status == HttpStatusCode.OK);
            }

            JsonNode transactions = ParseJsonFromResponseBody(get).FindPath("transactions");
            Assert.IsNotNull("JsonNode 'transactions' should not be null", transactions);
            Assert.AreEqual(1, transactions.size(), "There should be exactly 1 transaction");

            JsonNode operationNode = transactions.get(0).get("operation");
            Assert.AreEqual("Create", operationNode.asText());
            Assert.That(status == HttpStatusCode.OK);
        }

        private Transaction InitializeBasicTransaction(Agent agent)
        {
            Transaction tle = new Transaction(agent.GetAgentConfig());
            tle.SetFromKey("app01");
            tle.SetOperationKey("create");
            tle.SetToKey("esb");
            agent.AddTransaction(tle);
            return tle;
        }

        [Test]
        public void LoggerSendsDataToMockApiWithDelay()
        {
            Transaction tle = new Transaction(slowAinoAgent.GetAgentConfig());
            tle.SetOperationKey("create");
            tle.SetFromKey("app01");
            tle.SetToKey("esb");

            // Wait for logger to do one send iteration.
            Thread.Sleep(100);

            slowAinoAgent.AddTransaction(tle);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(API_URL + "/test/readTransactions");
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Assert.That(response.StatusCode == HttpStatusCode.OK);
            }

            JsonNode transactions = parseJsonFromResponseBody(get).findPath("transactions");
            Assert.IsNotNull("JsonNode 'transactions' should not be null", transactions);
            Assert.AreEqual(0, transactions.size(), "There should be 0 transaction");

            Thread.Sleep(5500); // logger send interval is 5000

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(API_URL + "/test/readTransactions");
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Assert.That(response.StatusCode == HttpStatusCode.OK);
            }


            transactions = parseJsonFromResponseBody(get).findPath("transactions");
            Assert.IsNotNull("JsonNode 'transactions' should not be null", transactions);
            Assert.AreEqual(1, transactions.size(), "There should be exactly 1 transaction");

        }

        [Test]
        public void LoggerSendsCorrectPayloadType()
        {
            Transaction tle = new Transaction(slowAinoAgent.GetAgentConfig());
            tle.SetOperationKey("update");
            tle.SetFromKey("app01");
            tle.SetToKey("esb");
            tle.SetPayloadTypeKey("subInterface01");

            ainoAgent.AddTransaction(tle);

            Thread.Sleep(2000);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(API_URL + "/test/readTransactions");
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Assert.That(response.StatusCode == HttpStatusCode.OK);
            }

            JsonNode transactions = parseJsonFromResponseBody(get).findPath("transactions");

            Assert.IsNotNull("JsonNode 'transactions' should not be null", transactions);
            Assert.AreEqual(1, transactions.size(), "There should be exactly 1 transaction");
            Assert.AreEqual("Update", transactions.findPath("operation").getTextValue(), "operation should be 'Update'");

            JsonNode payloadTypeNode = parseJsonFromResponseBody(get).findPath("payloadType");
            Assert.AreEqual("Interface 1", payloadTypeNode.getTextValue(), "payloadType should be 'Interface 1");
        }

        //Todo: This is now Jackson, change to Newtonsoft JSON
        private JsonNode ParseJsonFromResponseBody(HttpMethod method)
        {
            JsonParser jsonParser = new JsonFactory().createJsonParser(method.getResponseBody());
            ObjectMapper mapper = new ObjectMapper();
            return mapper.readTree(jsonParser);
        }
        */
    }
}
