using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aino.Agents.Core
{
    /// <summary>
    /// Interface for observing buffer size changes.
    /// </summary>
    public interface ITransactionDataObserver
    {
        // Called when new data is added to the observed buffer.
        // newSize is new size of the buffer

        void LogDataAdded(int newSize);
    }
}
