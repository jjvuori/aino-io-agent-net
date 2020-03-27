using Aino.Agents.Core.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aino.Agents.Core
{
    /// <summary>
    /// Class for creating log entries
    /// Once log entry is created with desired data, it should be passed to Agent for sending.
    /// </summary>
    public class Transaction
    {
        private string toKey;
        private string fromKey;
        private string operationKey;
        private string payloadTypeKey;
        private string message;
        private string status;
        private string flowId;
        private Dictionary<String, List<String>> ids;
        private long timestamp;
        private List<NameValuePair> metadata;
        private AgentConfig config;

        public sealed class FieldEnum
        {
            public enum FieldEnumMembers
            {
                to,
                from,
                operation,
                message,
                status,
                timestamp,
                payloadType,
                flowId
            }

            /*
            public static readonly string to;
            public static readonly string from;
            public static readonly string operation;
            public static readonly string message;
            public static readonly string status;
            public static readonly long timestamp;
            public static readonly string payloadType;
            public static readonly string flowId;
            */

            public static string fieldName;
            FieldEnum(string n)
            {
                fieldName = n;
            }

            public string GetFieldEnumName()
            {
                return fieldName;
            }
        }

        /// <summary>
        /// Gets value of field.
        /// </summary>
        /// <param name="field">Field type to get the value for</param>
        /// <returns>Value of the field</returns>
        public object GetFieldValue(FieldEnum field)
        {
           string fieldenum = field.ToString();
           switch (fieldenum)
            {
                case "to":
                    return config.GetApplications().GetEntry(GetToKey());
                case "from":
                    return config.GetApplications().GetEntry(GetFromKey());
                case "operation":
                    return config.GetOperations().GetEntry(GetOperationKey());
                case "message":
                    return GetMessage();
                case "status":
                    return GetStatus();
                case "timestamp":
                    return timestamp;
                case "payloadtype":
                    return config.GetPayloadTypes().GetEntry(GetPayloadTypeKey());
                case "flowid":
                    return GetFlowId();
                default:
                    throw new AgentCoreException("Invalid field [" + fieldenum + "]!");
            }
        }

        /// <summary>
        /// Constructor
        /// AgentConfig is used to get human readable values for keys.
        /// </summary>
        /// <param name="config">Agent configuration</param>
        public Transaction(AgentConfig config)
        {
            ids = new Dictionary<string, List<string>>();
            timestamp = DateTimeOffset.Now.Millisecond;
            metadata = new List<NameValuePair>(2);
            this.config = config;
        }

        /// <summary>
        /// Gets 'to application' key.
        /// </summary>
        /// <returns>key</returns>
        public string GetToKey()
        {
            return toKey;
        }

        /// <summary>
        /// Sets 'to application' key.
        /// </summary>
        /// <param name="toKey">key</param>
        public void SetToKey(string toKey)
        {
            this.toKey = toKey;
        }

        /// <summary>
        /// Gets 'from application' key.
        /// </summary>
        /// <returns>key</returns>
        public string GetFromKey()
        {
            return fromKey;
        }

        /// <summary>
        /// Sets 'from application' key.
        /// </summary>
        /// <param name="fromKey">key</param>
        public void SetFromKey(string fromKey)
        {
            this.fromKey = fromKey;
        }

        /// <summary>
        /// Gets 'operation' key.
        /// </summary>
        /// <returns>key</returns>
        public string GetOperationKey()
        {
            return operationKey;
        }

        /// <summary>
        /// Sets 'operation' key.
        /// </summary>
        /// <param name="operationKey">key</param>
        public void SetOperationKey(string operationKey)
        {
            this.operationKey = operationKey;
        }

        /// <summary>
        /// Gets 'payload type' key.
        /// </summary>
        /// <returns>key</returns>
        public string GetPayloadTypeKey()
        {
            return payloadTypeKey;
        }

        /// <summary>
        /// Sets 'payload type' key.
        /// </summary>
        /// <param name="payloadTypeKey">key</param>
        public void SetPayloadTypeKey(string payloadTypeKey)
        {
            this.payloadTypeKey = payloadTypeKey;
        }

        /// <summary>
        /// Gets message.
        /// </summary>
        /// <returns>message</returns>
        public string GetMessage()
        {
            return message;
        }

        /// <summary>
        /// Sets message.
        /// </summary>
        /// <param name="message">message</param>
        public void SetMessage(string message)
        {
            this.message = message;
        }

        /// <summary>
        /// Gets status.
        /// Possible values are 'failure', 'success', 'unknown'.
        /// </summary>
        /// <returns>status</returns>
        public string GetStatus()
        {
            return status;
        }

        /// <summary>
        /// Sets status.
        /// Possible values are 'failure', 'success', 'unknown'.
        /// </summary>
        /// <param name="status">"failure", "success", "unknown"</param>
        public void SetStatus(string status)
        {
            this.status = status;
        }

        /// <summary>
        /// Gets the flow id.
        /// Also known as correlation id.
        /// </summary>
        /// <returns>Flow id</returns>
        public string GetFlowId()
        {
            return flowId;
        }

        /// <summary>
        /// Sets the flow id.
        /// Also known as correlation id.
        /// </summary>
        /// <param name="flowId">Flow id</param>
        public void SetFlowId(string flowId)
        {
            this.flowId = flowId;
        }

        /// <summary>
        /// Gets the timestamp this entry was created.
        /// </summary>
        /// <returns>timestamp</returns>
        public long GetTimestamp()
        {
            return timestamp;
        }

        /// <summary>
        /// Gets all ids passed to this entry.
        /// </summary>
        /// <returns>ids</returns>
        public Dictionary<string, List<string>> GetIds()
        {
            return ids;
        }

        /// <summary>
        /// Gets id type name for key.
        /// </summary>
        /// <param name="key">key of id type</param>
        /// <returns>name corresponding to key</returns>
        public string GetIdTypeName(string key)
        {
            return config.GetIdTypes().GetEntry(key);
        }

        /// <summary>
        /// Gets all ids in this entry based on type key.
        /// </summary>
        /// <param name="typeKey">key</param>
        /// <returns>All ids</returns>
        public List<string> GetIdsByType(string typeKey)
        {
            return ids[typeKey];
        }

        /// <summary>
        /// Adds id type key.
        /// </summary>
        /// <param name="typeKey">Key to add</param>
        /// <returns>List for ids</returns>
        public List<string> AddIdTypeKey(string typeKey)
        {
            ids.Add(typeKey, new List<string>());
            return new List<string>();
        }

        /// <summary>
        /// Adds ids for type key.
        /// </summary>
        /// <param name="typeKey">Key of id type</param>
        /// <param name="ids">Ids to add for typeKey</param>
        /// <returns>List of ids</returns>
        public List<string> AddIdsByTypeKey(string typeKey, List<string> ids)
        {
            List<string> list = GetIdsByType(typeKey);
            if (list == null)
            {
                list = new List<string>();
                this.ids.Add(typeKey, list);
            }
            list.AddRange(ids);

            return list;
        }

        /// <summary>
        /// Adds metadata to this entry.
        /// </summary>
        /// <param name="key">Key of metadata</param>
        /// <param name="value">Value of metadata</param>
        public void AddMetadata(string key, string value)
        {
            NameValuePair toRemove = null;
            foreach (NameValuePair nvp in metadata)
            {
                if (string.Equals(nvp.Name, key))
                {
                    toRemove = nvp;
                }
            }
            if (toRemove != null) this.metadata.Remove(toRemove);

            metadata.Add(new NameValuePair(key, value));
        }

        /// <summary>
        /// Gets all metadata from this entry.
        /// </summary>
        /// <returns>All metadata</returns>
        public List<NameValuePair> GetMetadata()
        {
            return metadata;
        }
    }
}