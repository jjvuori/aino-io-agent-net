using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using Newtonsoft.Json;

namespace Aino.Agents.Core
{
    /// <summary>
    /// Buffer for holding TransactionSerializable objects to be sent.
    /// </summary>
    public class TransactionDataBuffer
    {
        private readonly List<ITransactionDataObserver> observers = new List<ITransactionDataObserver>();

        private readonly ConcurrentQueue<TransactionSerializable> transactions = new ConcurrentQueue<TransactionSerializable>();
        private static readonly object _lock = new object();

        private readonly int sizeThreshold;

        /// <summary>
        /// Adds serializable version log entry to the buffer.
        /// </summary>
        /// <param name="entry">Serializable log entry</param>
        public void AddTransaction(TransactionSerializable entry)
        {

            this.transactions.Enqueue(entry);

            if (Monitor.TryEnter(_lock))
            {
                int currentSize = this.GetSize();
                try
                {
                    int observerCount = observers.Count;

                    for (int i = 0; i < observerCount; i++)
                    {
                        ITransactionDataObserver observer = observers.ElementAt(i);
                        if (observer != null)
                        {
                            observer.LogDataAdded(currentSize);
                        }
                    }

                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }
        }

        /// <summary>
        /// Gets the number of entries in this buffer.
        /// </summary>
        /// <returns>Entry count</returns>
        public int GetSize()
        {
            return transactions.Count;
        }

        public TransactionDataBuffer(int sizeThreshold)
        {
            this.sizeThreshold = sizeThreshold;
        }

        
        /// <summary>
        /// Adds observer for listening to size changes in buffer.
        /// </summary>
        /// <param name="observer">Observer</param>
        public void AddLogDataSizeObserver(ITransactionDataObserver observer)
        {
            observers.Add(observer);
        }

        /// <summary>
        /// Checks if this buffer is empty. 
        /// </summary>
        /// <returns>Whether is empty</returns>
        public bool IsEmpty()
        {
            return transactions.IsEmpty;
        }

        /// <summary>
        /// Checks if this buffer contains entries.
        /// /// </summary>
        /// <returns>Whether contains data</returns>
        public bool ContainsData()
        {
            return !transactions.IsEmpty;
        }

        private int ElementsToDrain()
        {
            // ensure transactions get sent one at a time when size threshold is zero or one
            return sizeThreshold <= 1 ? 1 : int.MaxValue;
        }


        /// <summary>
        /// Returns the entries serialized as string and clears this buffer.
        /// </summary>
        /// <returns>Serializable log entries</returns>
        /// <exception cref="IOException">When json serialization fails</exception>
        public string GetDataToSend()
        {
            List<TransactionSerializable> entries = new List<TransactionSerializable>();

            int elementstodrain = ElementsToDrain();
            for (int entryindex = 0; entryindex < elementstodrain; entryindex++)
            {
                if(transactions.TryDequeue(out TransactionSerializable ts))
                {
                    entries.Add(ts);
                }
            }

            string output = JsonConvert.SerializeObject(entries);
            return output;
        }
    }
}
