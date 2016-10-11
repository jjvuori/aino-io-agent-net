using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Aino
{
    public class MessageQueue 
    {

        private readonly List<AinoMessage> _messages = new List<AinoMessage>(); 
        private readonly object _lock = new object();

        public void Enqueue(AinoMessage msg)
        {
            lock (_lock)
            {
                _messages.Add(msg);
            }
        }

        public int Count
        {
            get { return _messages.Count; }
        }

        public List<AinoMessage> DequeueAll()
        {
            lock (_lock)
            {
                var data = new AinoMessage[_messages.Count];
                _messages.CopyTo(data);
                _messages.Clear();

                return new List<AinoMessage>(data);
            }
        }

        public bool IsEmpty
        {
            get { return _messages.Count == 0; }
        }

        public void ToJson(Stream stream)
        {
            var data = new SerializationArray {Transactions = DequeueAll()};

            new DataContractJsonSerializer(typeof(SerializationArray)).WriteObject(stream, data);
        }

        [DataContract]
        protected class SerializationArray
        {
            [DataMember(Name = "transactions")]
            public IList<AinoMessage> Transactions = new List<AinoMessage>();
        }
    }
}
