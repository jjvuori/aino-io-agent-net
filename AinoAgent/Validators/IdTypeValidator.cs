using Aino.Agents.Core.Config;

namespace Aino.Agents.Core.Validators
{
    /// <summary>
    /// Validator for <see cref="Transaction"/>'s id types.
    /// Checks that log entry's id types are configured to the agent.
    /// </summary>
    class IdTypeValidator : ITransactionValidator
    {
        private AgentConfig config;

        /// <summary>
        /// Constructor.
        /// /// </summary>
        /// <param name="agentConfig">Agent configuration</param>
        public IdTypeValidator(AgentConfig agentConfig)
        {
            config = agentConfig;
        }

        public void Validate(Transaction entry)
        {
            foreach(string val in entry.GetIds().Keys)
            {
                if (!config.GetIdTypes().EntryExists(val))
                {
                    throw new AgentCoreException("IdType not found: " + val);
                }
            }
        }
    }
}