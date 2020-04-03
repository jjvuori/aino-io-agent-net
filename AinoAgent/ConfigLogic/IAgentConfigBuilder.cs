using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aino.Agents.Core.Config
{
    public interface IAgentConfigBuilder
    {
        // Builds the configuration object.
        // Returns configuration object
        AgentConfig Build();
    }
}
