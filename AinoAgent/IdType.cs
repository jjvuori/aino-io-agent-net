using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AinoAgent
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
