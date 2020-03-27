using Aino.Agents.Core.Config;

namespace Aino.Agents.Core.Validators
{
    class ApplicationValidator : ITransactionValidator
    {
        private AgentConfig config;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="conf">Agent configuration</param>
        public ApplicationValidator(AgentConfig conf)
        {
            config = conf;
        }

        public void Validate(Transaction entry)
        {
            ValidateFromToExists(entry);
            ValidateFromApplication(entry);
            ValidateToApplication(entry);
        }

        private void ValidateFromToExists(Transaction entry)
        {
            if (string.IsNullOrWhiteSpace(entry.GetFromKey()))
            {
                throw new AgentCoreException("from does not exist!");
            }
            if (string.IsNullOrWhiteSpace(entry.GetToKey()))
            {
                throw new AgentCoreException("to does not exist!");
            }
        }

        private void ValidateFromApplication(Transaction entry)
        {
            if (!config.GetApplications().EntryExists(entry.GetFromKey()))
            {
                throw new AgentCoreException("from application does not exist: " + entry.GetFromKey());
            }
        }

        private void ValidateToApplication(Transaction entry)
        {
            if (!config.GetApplications().EntryExists(entry.GetToKey()))
            {
                throw new AgentCoreException("to application does not exist: " + entry.GetToKey());
            }
        }

    }
}
