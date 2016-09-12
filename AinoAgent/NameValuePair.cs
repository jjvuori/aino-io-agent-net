using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AinoAgent
{
    // For controlling the serialization.
    [DataContract]
    public class NameValuePair
    {
        [DataMember(Name = "name")] public string Name;
        [DataMember(Name = "value")] public string Value;

        public NameValuePair(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
