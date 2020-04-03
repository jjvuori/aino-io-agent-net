using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aino
{
    public class Configuration
    {
        public string ApiAddress { get; set; } = ConfigurationManager.AppSettings["ApiAddress"];
        public string ApiKey { get; set; } = ConfigurationManager.AppSettings["ApiKey"];
        public int SendInterval { get; set; } = int.Parse(ConfigurationManager.AppSettings["SendInterval"]);
        public int SizeThreshold { get; set; } = int.Parse(ConfigurationManager.AppSettings["SizeThreshold"]);
        public bool Gzip { get; set; } = bool.Parse(ConfigurationManager.AppSettings["Gzip"]);
    }
}
