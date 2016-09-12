using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Aino
{
    [DataContract]
    public class IdType
    {

        [DataMember(Name = "idType")]
        public string IdTypeName { get; set; }

        [DataMember(Name = "values")]
        public List<string> Ids { get; set; }

        public IdType()
        {
            Ids = new List<string>();
        }

    }
}
