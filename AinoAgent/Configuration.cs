using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aino
{
    public class Configuration
    {
        public string ApiAddress { get; set; } = "https://data.aino.io/rest/v2.0/transaction";
        public string ApiKey { get; set; }
        public int SendInterval { get; set; } = 15000;
        public int SizeThreshold { get; set; } = 10;
        public bool Gzip { get; set; } = false;
    }
}
