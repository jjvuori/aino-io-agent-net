using System;

namespace Aino.Agents.Core
{
    /// <summary>
    /// Exception for runtime agent errors.
    /// Includes validation errors etc.
    /// </summary>
    public class AgentCoreException : Exception
    {
        public AgentCoreException(String message) : base(message)
        {
        }

        public AgentCoreException(String message, Exception cause) : base(message, cause)
        {
        }
    }
}
