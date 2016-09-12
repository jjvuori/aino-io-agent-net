using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace AinoAgent
{
    [DataContract]
    public class AinoMessage
    {

        public AinoMessage()
        {
            Ids = new List<IdType>();
            Metadata = new List<NameValuePair>();
        }

        [DataMember(Name = "timestamp", IsRequired = true)]
        public DateTime Timestamp { get; set; }

        [DataMember(Name = "from", IsRequired = true)]
        public string From { get; set; }

        [DataMember(Name = "to", IsRequired = true)]
        public string To { get; set; }

        //[DataMember(Name = "status", IsRequired = true)]
        public MessageStatus Status { get; set; }

        [DataMember(Name = "status")]
        public string StatusString
        {
            get { return Status.ToString(); }
            private set { }
        }


        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "operation")]
        public string Operation { get; set; }

        [DataMember(Name = "payloadType")]
        public string PayloadType { get; set; }

        [DataMember(Name = "flowId")]
        public string FlowId { get; set; }

        [DataMember(Name = "ids")]
        public List<IdType> Ids { get; private set; }

        [DataMember(Name = "metadata")]
        public List<NameValuePair> Metadata { get; private set; }


        [DataContract]
        public enum MessageStatus
        {
            [EnumMember(Value = "unknown")]
            Unknown,

            [EnumMember(Value = "success")]
            Success,


            [EnumMember(Value = "failure")]
            Failure
        }

        public void AddId(string idKey, string idValue)
        {
            var foundType = Ids.Find(o => o.IdTypeName == idKey);
            if (foundType != null)
            {
                foundType.Ids.Add(idValue);
            }
            else
            {
                var idType = new IdType {IdTypeName = idKey};
                idType.Ids.Add(idValue);
                Ids.Add(idType);
            }
        }

        public void AddId(string idKey, List<string> idValues)
        {
            var foundType = Ids.Find(o => o.IdTypeName == idKey);
            if (foundType != null)
            {
                foundType.Ids.AddRange(idValues);
            }
            else
            {
                var idType = new IdType {IdTypeName = idKey};
                idType.Ids.AddRange(idValues);
                Ids.Add(idType);
            }
        }

        public void AddMetadata(string key, string value)
        {
            if (Metadata == null)
            {
                Metadata = new List<NameValuePair>();
            }

            Metadata.Add(new NameValuePair(key, value));
        }

        public string ToJson()
        {
            var settings = new DataContractJsonSerializerSettings { DateTimeFormat = new DateTimeFormat("o") };
            using (var stream = new MemoryStream())
            {
                new DataContractJsonSerializer(typeof(AinoMessage), settings).WriteObject(stream, this);
                stream.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public void ToJson(Stream stream)
        {
            
        }

    }
}
