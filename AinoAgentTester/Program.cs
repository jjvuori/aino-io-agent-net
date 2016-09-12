using System;
using System.Collections.Generic;
using AinoAgent;

namespace AinoAgentTester
{
    class Program
    {

        static void Main(string[] args)
        {
            AinoMessage msg = new AinoMessage();

            msg.From = "CSharp agent";
            msg.To = "Unknown!";
            msg.Timestamp = DateTime.Now;
            msg.AddMetadata("MetadataKey1", "MetadataValue1");
            msg.AddMetadata("meta 2", "meta 2 value");

            msg.AddId("single value", "value1");
            msg.AddId("single value", "value 2");

            msg.AddId("Multiple values 1", new List<string> {"val1", "val2", "val3"});
            msg.AddId("Multiple values 2", new List<string> { "val11", "val22", "val33"});
            msg.AddId("Multiple values 2", new List<string> {"val44", "val55"});
            msg.Status = AinoMessage.MessageStatus.Success;

            Console.WriteLine(msg.ToJson());

            Console.ReadLine();

        }
    }
}
