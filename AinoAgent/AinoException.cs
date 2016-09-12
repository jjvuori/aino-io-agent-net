using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aino
{
    public class AinoException : Exception
    {
        public AinoException(string message) : base(message)
        {
        }
    }
}
