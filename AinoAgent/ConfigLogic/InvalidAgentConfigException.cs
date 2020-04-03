using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aino.Agents.Core.Config
{
    /// <summary>
    /// Exception for invalid agent configuration.
    /// </summary>
    public class InvalidAgentConfigException : Exception
    {
        private const long SERIAL_VERSION_UID = -5242222476445646150L;

        public InvalidAgentConfigException(string message) : base(message)
        {
        }

        public InvalidAgentConfigException(string message, Exception cause) : base(message, cause)
        {
        }
    }
}
