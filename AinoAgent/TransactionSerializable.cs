using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//Todo: Linked Hash Map doesn't exist in C#, so it needs to be tested for the right order!

namespace Aino.Agents.Core
{
    /// <summary>
    /// This class exists for convenient JSON serialization (by Newtonsoft) of log messages.
    /// Should not be used directly. Use Transaction instead.
    /// </summary>
    public class TransactionSerializable
    {
        /// <summary>
        /// Creates TransactionSerializable from Transaction.
        /// </summary>
        /// <param name="entry">Log entry to create from</param>
        /// <returns>Created TransactionSerializable</returns>
        public static TransactionSerializable From(Transaction entry)
        {
            TransactionSerializable obj = new TransactionSerializable();

            foreach (var field in Enum.GetValues(typeof(Transaction.FieldEnum.FieldEnumMembers)))
            {
                try
                {
                    string fe = field.ToString();
                    var fv = entry.GetFieldValue(fe);
                    obj.AddField(field.ToString(), fv);
                }
                catch { }
            }

            Dictionary<string, List<string>> g = entry.GetIds();
            foreach (KeyValuePair<string, List <string>> idList in g)
            {
                IdList list = obj.AddIdType(entry.GetIdTypeName(idList.Key));
                list.AddIds(idList.Value);
            }

            obj.SetMetadata(entry.GetMetadata());

            return obj;
        }

        public class IdList
        {
            private string idType;
            private readonly List<string> values = new List<string>();

            public IdList()
            {
            }

            public string GetIdType() => idType;

            public void SetIdType(string idType) => this.idType = idType;

            public void AddId(string id) => values.Add(id);

            public void AddIds(List<string> ids) => values.AddRange(ids);

            public List<string> GetValues() => values;
        }

        private int size;

        public readonly Dictionary<string, object> fields = new Dictionary<string, object>();

        private readonly Dictionary<string, IdList> idLists = new Dictionary<string, IdList>();


        /// <summary>
        /// Constructor.
        /// Sets timestamp.
        /// </summary>
        public TransactionSerializable()
        {
            fields.Add("timestamp", DateTime.Now.Ticks);
            fields.Add("ids", new List<IdList>(2));
        }

        /// <summary>
        /// Add new id type.
        /// </summary>
        /// <param name="idType">Id type to add</param>
        /// <returns>List for id type</returns>
        public IdList AddIdType(string idType)
        {
            IdList idList = new IdList();
            idList.SetIdType(idType);

            if (idLists.ContainsKey(idType))
            {
                throw new Exception("Duplicate IdList in a TransactionSerializable.");
            }

            idLists.Add(idType, idList);

            List<IdList> idsField = (List<IdList>)fields["ids"];
            idsField.Add(idList);

            return idList;
        }

        /// <summary>
        /// Adds metadata.
        /// </summary>
        /// <param name="data">Metadata to add</param>
        public void SetMetadata(List<NameValuePair> data) => fields.Add("metadata", data);

        private object GetField(string key)
        {
            object value = fields[key];
            if (value == null)
            {
                value = "";
            }
            return value;
        }

        private string GetFieldAsString(string key)
        {
            string value = (string)fields[key];
            if (value == null)
            {
                value = "";
            }
            return value;
        }

        /// <summary>
        /// Adds field.
        /// </summary>
        /// <param name="name">Name of the field</param>
        /// <param name="value">Value of the field</param>
        public void AddField(string name, object value) => fields.Add(name, value);

        /// <summary>
        /// Gets 'from' field.
        /// Used for serialization.
        /// </summary>
        /// <returns>Field value</returns>
        public string GetFrom() => GetFieldAsString("from");

        /// <summary>
        /// Gets 'to' field.
        /// Used for serialization.
        /// </summary>
        /// <returns>Field value</returns>
        public string GetTo() => GetFieldAsString("to");

        /// <summary>
        /// Gets 'message' field.
        /// Used for serialization.
        /// </summary>
        /// <returns>Field value</returns>
        public string GetMessage() => GetFieldAsString("message");

        /// <summary>
        /// Gets 'operation' field.
        /// Used for serialization.
        /// </summary>
        /// <returns>Field value</returns>
        public string GetOperation() => GetFieldAsString("operation");

        /// <summary>
        /// Gets 'flowId' field.
        /// Used for serialization.
        /// </summary>
        /// <returns>Field value</returns>
        public string GetFlowId() => GetFieldAsString("flowId");

        /// <summary>
        /// Gets 'timestamp' field.
        /// Used for serialization.
        /// </summary>
        /// <returns>Field value</returns>
        public long GetTimestamp() => (long)GetField("timestamp");

        /// <summary>
        /// Gets 'status' field.
        /// Used for serialization.
        /// </summary>
        /// <returns>Field value</returns>
        public string GetStatus() => GetFieldAsString("status");

        /// <summary>
        /// Gets 'payloadType' field.
        /// Used for serialization.
        /// </summary>
        /// <returns>Field value</returns>
        public string GetPayloadType() => GetFieldAsString("payloadType");

        /// <summary>
        /// Gets 'ids' field.
        /// Used for serialization.
        /// </summary>
        /// <returns>Field value as List<IdList></returns>
        public List<IdList> GetIds() => (List<IdList>)GetField("ids");

        /// <summary>
        /// Gets 'metadata' field. Used for serialization.
        /// </summary>
        /// <returns>Field values as List<NameValuePair></returns>
        public List<NameValuePair> GetMetadata() => (List<NameValuePair>)GetField("metadata");

        [JsonIgnore]
        public int GetSize { get; }
        
        public void SetSize(int size)
        {
            this.size = size;
        }
    }
}
