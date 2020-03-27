using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Dynamic;
using Aino;
using Aino.Agents.Core;

namespace AinoAgentTester
{
    class Program
    {

        static void Main(string[] args)
        {
            using (Agent agent = new Agent())
            {
                agent.Configuration = GetAinoConfiguration();
                agent.Initialize();

                while (true)
                {
                    Console.ReadLine();
                    var msg = CreateMessage();
                    agent.AddMessage(msg);
                }
            }
        }

        static Aino.Configuration GetAinoConfiguration()
        {
            string apikey = ConfigurationManager.AppSettings.Get("ApiKey");
            if (!int.TryParse(ConfigurationManager.AppSettings.Get("SendInterval"), out int sendinterval))
            {
                throw new ArgumentException("SendInterval in config file is not integer");
            }

            if (!int.TryParse(ConfigurationManager.AppSettings.Get("SizeThreshold"), out int sizethreshold))
            {
                throw new ArgumentException("SizeThreshold in config file is not integer");
            }

            if (!bool.TryParse(ConfigurationManager.AppSettings.Get("GZip"), out bool gzip))
            {
                throw new ArgumentException("GZip in config file is not boolean");
            }

            var conf = new Aino.Configuration
            {
                ApiKey = apikey,
                SendInterval = sendinterval,
                SizeThreshold = sizethreshold,
                Gzip = gzip
            };

            return conf;
        }

        static AinoMessage CreateMessage()
        {
            AinoMessage msg = new AinoMessage
            {
                From = "System 0",
                To = "System 1",
                Timestamp = DateTime.Now
            };

            msg.AddMetadata("MetadataKey1", "MetadataValue1");
            msg.AddMetadata("meta 2", "meta 2 value");

            msg.AddId("single value", "value1");
            msg.AddId("single value", "value 2");

            msg.AddId("Multiple values 1", new List<string> { "val1", "val2", "val3" });
            msg.AddId("Multiple values 2", new List<string> { "val11", "val22", "val33" });
            msg.AddId("Multiple values 2", new List<string> { "val44", "val55" });
            msg.Status = AinoMessage.MessageStatus.Success;
            msg.Message = "Testing dotnet core!";

            return msg;
        }
    }
}
