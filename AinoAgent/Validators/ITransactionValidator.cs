namespace Aino.Agents.Core.Validators
{
    /// <summary>
    /// Interface for <see cref="Transaction"/> validators.
    /// </summary>
    interface ITransactionValidator
    {
        /// <summary>
        /// Do the validation.
        /// </summary>
        /// <param name="entry">Log entry to validate</param>
        /// <exception cref="AgentCoreException">When Validation fails</exception>
        void Validate(Transaction entry);
    }
}
