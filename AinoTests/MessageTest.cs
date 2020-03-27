using System;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using static Aino.AinoMessage;
using Aino;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;
using System.Runtime.Serialization;

namespace AinoTests
{
	[TestFixture]
	public class MessageTest
	{

        [Test]
	    public void TestSerializationThrowsWithoutRequiredFields()
	    {
	        var msg = new AinoMessage {From = "from field!"};

            Assert.Throws<Aino.AinoException>(delegate { msg.ToJson(); });            
	    }

		[Test]
		public void TestAinoSerialization()
		{
			var ainoMessage = new AinoMessage
			{
				From = "From 1",
				To = "To",
				Status = MessageStatus.Failure
			};

			string messageJson = ainoMessage.ToJson();

			Assert.IsTrue(messageJson.Contains("\"from\":\"From 1\""), "Message contains correct 'from' field");
			Assert.IsTrue(messageJson.Contains("\"to\":\"To\""), "Message contains correct 'to' field");
			Assert.IsTrue(messageJson.Contains("\"status\":\"failure\""), "Message contains correct 'status' field");
		}

        [Test]
	    public void TestAinoTimestampSerialization()
        {
            var ainoMessage = new AinoMessage
            {
                To = "asdf",
                From = "asdf"
            };

            var expectedTimestamp = ainoMessage.Timestamp.ToString("o");

            var messageJson = ainoMessage.ToJson();

            Assert.IsTrue(messageJson.Contains("\"timestamp\":\"" + expectedTimestamp + "\""), "Message contains correctly formatted timestamp");
        }

        [Test]
	    public void TestAddingMetadataWorks()
	    {
	        var ainoMessage = new AinoMessage
	        {
                To = "to",
                From = "from",
                Status = MessageStatus.Success
	        };

            ainoMessage.AddMetadata("field1", "metadata value 1");

	        Assert.AreEqual("field1", ainoMessage.Metadata.First().Name, "Metadata key is correct");
	        Assert.AreEqual("metadata value 1", ainoMessage.Metadata.First().Value, "Metadata value is correct");

            ainoMessage.AddMetadata("field2", "metadata val 2");

	        Assert.AreEqual(2, ainoMessage.Metadata.Count, "Metadata count is correct");
	    }

	    [Test]
	    public void TestAddingIdsWork()
	    {
	        var ainoMessage = new AinoMessage
	        {
	            To = "to",
                From = "from"
	        };

            ainoMessage.AddId("idKey1", "value 1");
            ainoMessage.AddId("idKey1", "value 2");
            ainoMessage.AddId("idKey3", "value 5");

            Assert.IsTrue(ainoMessage.Ids.Exists(o => o.IdTypeName.Equals("idKey1")), "Valid Id key is found.");
            Assert.IsTrue(ainoMessage.Ids.Exists(o => o.IdTypeName.Equals("idKey3")), "Valid Id key is found.");
            Assert.IsFalse(ainoMessage.Ids.Exists(o => o.IdTypeName.Equals("idKey2")), "Invalid Id key is not found.");

	        var ids1 = ainoMessage.Ids.Find(o => o.IdTypeName.Equals("idKey1"));
            var ids2 = ainoMessage.Ids.Find(o => o.IdTypeName.Equals("idKey3"));

            Assert.IsTrue(ids1.Ids.Exists(o => o.Equals("value 1")), "Correct id value is found.");
            Assert.IsTrue(ids1.Ids.Exists(o => o.Equals("value 2")), "Correct id value is found.");
            Assert.IsTrue(ids2.Ids.Exists(o => o.Equals("value 5")), "Correct Id value is found.");
            Assert.IsFalse(ids1.Ids.Exists(o => o.Equals("value 5")), "Invalid Id value is not found.");

        }

        [Test]
	    public void TestEnumSerialization()
        {
            var ainoMessage = new AinoMessage
            {
                To = "to",
                From = "from",
                Status = MessageStatus.Failure
            };

            checkThatMessageContainsStatus(ainoMessage, "failure");

            ainoMessage.Status = MessageStatus.Success;
            checkThatMessageContainsStatus(ainoMessage, "success");

            ainoMessage.Status = MessageStatus.Unknown;
            checkThatMessageContainsStatus(ainoMessage, "unknown");
        }

	    private void checkThatMessageContainsStatus(AinoMessage msg, string status)
	    {
            Assert.IsTrue(msg.ToJson().Contains("\"status\":\"" + status + "\""));
	    }

	}
}

