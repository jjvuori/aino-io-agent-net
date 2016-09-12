using System;
using System.Collections.Generic;
using System.Dynamic;
using Aino;

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




        static Configuration GetAinoConfiguration()
        {
            var conf = new Configuration
            {
                ApiKey = "67437716-ed3c-4f6b-84eb-a6b5dd560f3c",
                SendInterval = 5000,
                SizeThreshold = 3,
                Gzip = true
            };

            return conf;
        }

        static AinoMessage CreateMessage()
        {
            AinoMessage msg = new AinoMessage();

            msg.From = "System 0";
            msg.To = "System 1";
            msg.Timestamp = DateTime.Now;
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
