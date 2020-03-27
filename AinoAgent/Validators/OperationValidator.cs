using Aino.Agents.Core.Config;

namespace Aino.Agents.Core.Validators
{
    /// <summary>
    /// Validator for <see cref="Transaction"/>'s operation.
    /// Checks that operation is configured to the agent.
    /// </summary>
    class OperationValidator : ITransactionValidator
    {
        private AgentConfig config;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="conf">Agent configuration</param>
        public OperationValidator(AgentConfig conf)
        {
            config = conf;
        }

        public void Validate(Transaction entry)
        {
            if (null != entry.GetOperationKey() && !config.GetOperations().EntryExists(entry.GetOperationKey()))
                throw new AgentCoreException("Operation does not exist: " + entry.GetOperationKey());
        }
    }
}
