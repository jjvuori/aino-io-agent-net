using System.Runtime.Serialization;

namespace Aino
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
