using System;
using System.Collections.Generic;
using System.Threading;
using Aino.Agents.Core.Config;
using Aino.Agents.Core.OverloadChecker;
using Aino.Agents.Core.Validators;
using log4net;


namespace Aino.Agents.Core
{

    delegate void DataAdded(int size);

    public class Agent : IThreadAmountObserver, IDisposable
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(Agent));

        private Thread _senderThread;
        private HttpSender _sender;
        private Sender _threadsender;
        private readonly MessageQueue _messages;
        private DataAdded _dataDelegates;
        private Dictionary<Thread, Sender> senderThreads = new Dictionary<Thread, Sender>();
        private TransactionDataBuffer dataBuffer;
        private List<ITransactionValidator> validators;
        private Timer overloadCheckerTimer;
        private readonly int MAX_THREAD_AMOUNT = 5;

        private readonly AgentConfig agentConfig;

        public Configuration Configuration { get; set; }

        bool disposed;

        private Agent(AgentConfig config)
        {
            this.agentConfig = config;


            dataBuffer = new TransactionDataBuffer(agentConfig.GetSizeThreshold());
            //Sender sender = CreateSender();
            //senderThreads.Add(new Thread(sender), sender);


            Thread thread = new Thread(new ThreadStart(AddSenderThread));
            senderThreads.Add(thread, _threadsender);


            validators = new List<ITransactionValidator>();
            validators.Add(new OperationValidator(this.agentConfig));
            validators.Add(new IdTypeValidator(this.agentConfig));
            validators.Add(new ApplicationValidator(this.agentConfig));

            overloadCheckerTimer = new Timer(obj =>
            {
                new SenderOverloadCheckerTask(this, dataBuffer, agentConfig);
            }, null, TimeSpan.FromMilliseconds(5000), TimeSpan.FromMilliseconds(5000));

            if (IsEnabled())
            {
                _logger.Info("Aino logger is enabled, starting sender thread.");
                foreach (Thread threaditerator in senderThreads.Keys)
                {
                    threaditerator.Start();
                }
            }
            _logger.Info("Aino logger initialized.");
        }

        public void AddSenderThread()
        {
            Sender sender = CreateSender();
            _threadsender = sender;
        }

        public Agent()
        {
            _logger.Info("Aino Agent created");
            _messages = new MessageQueue();
        }

        public int GetSenderThreadCount()
        {
            return this.senderThreads.Count;
        }

        public void Initialize()
        {
            // RAII ??!!??! 
            if (Configuration == null)
            {
                _logger.Fatal("Configuration object not found. Throwing exception...");
                throw new AinoException("Could not initialize. Configuration missing.");
            }

            _sender = new HttpSender(_messages, Configuration);
            _dataDelegates += _sender.DataAdded;
            _senderThread = new Thread(_sender.StartSending);
            _logger.Info("Starting sender thread");
            _senderThread.Start();
        }

        /// <summary>
        /// Gracefully stop the sender threads.
        /// </summary>
        public void Stop()
        {
            overloadCheckerTimer.Change(Timeout.Infinite, Timeout.Infinite);

            foreach (Thread threaditerator in senderThreads.Keys)
            {
                threaditerator.Abort();
            }

            foreach (Thread threaditerator in senderThreads.Keys)
            {
                try
                {
                    threaditerator.Join();
                }
                catch (ThreadInterruptedException ignored)
                {
                    _ = ignored.InnerException.ToString();
                }
            }

            senderThreads.Clear();
        }

        /// <summary>
        /// Alias for {@link #stop()}
        /// </summary>
        public void Shutdown()
        {
            this.Stop();
        }

        /// <summary>
        /// Returns new entry.
        /// Returned Transaction should be populated with applications, operations, etc before passing
        /// to {@link #addTransaction(Transaction)}.
        /// </summary>
        /// <returns>New Transaction for logging</returns>
        public Transaction NewTransaction()
        {
            return new Transaction(this.agentConfig);
        }

        /// <summary>
        /// Adds log entry to be sent to aino.io. 
        /// </summary>
        /// <param name="entry">Log entry to be sent</param>
        public void AddTransaction(Transaction entry)
        {
            if (!IsEnabled())
            {
                return;
            }
            ValidateTransaction(entry);
            TransactionSerializable les = TransactionSerializable.From(entry);
            dataBuffer.AddTransaction(les);
            _logger.Debug("Added log entry.");
        }

        private void ValidateTransaction(Transaction trans)
        {
            foreach (ITransactionValidator validator in this.validators)
            {
                validator.Validate(trans);
            }
        }

        private Sender CreateSender()
        {
            return new Sender(this.agentConfig, this.dataBuffer, new DefaultApiClient(this.agentConfig));
        }


        /// <summary>
        /// Checks if this agent is enabled.
        /// </summary>
        /// <returns>true if enabled</returns>
        public bool IsEnabled()
        {
            return this.agentConfig.IsEnabled();
        }

        public void DecreaseThreads()
        {
            _logger.Info("Not implemented yet!");
        }

        /// <summary>
        /// Checks if application key is configured to this agent.
        /// </summary>
        /// <param name="operationKey">Operation key to check</param>
        /// <returns>Whether key was found</returns>
        public bool ApplicationExists(string applicationKey)
        {
            return this.agentConfig.GetApplications().EntryExists(applicationKey);
        }

        /// <summary>
        /// Checks if operation key is configured to this agent.
        /// </summary>
        /// <param name="operationKey">Operation key to check</param>
        /// <returns>Whether key was found</returns>
        public bool OperationExists(string operationKey)
        {
            return this.agentConfig.GetOperations().EntryExists(operationKey);
        }

        /// <summary>
        /// Checks if payload type key is configured to this agent.
        /// </summary>
        /// <param name="payloadTypeKey">Payload type key to check</param>
        /// <returns>Whether key was found</returns>
        public bool PayloadTypeExists(string payloadTypeKey)
        {
            return this.agentConfig.GetPayloadTypes().EntryExists(payloadTypeKey);
        }

        /// <summary>
        /// Gets the configration object for this agent.
        /// </summary>
        /// <returns>Configuration object</returns>
        public AgentConfig GetAgentConfig()
        {
            return this.agentConfig;
        }

        /// <summary>
        /// Gets factory for creating agent.
        /// </summary>
        /// <returns>LoggerFactory</returns>
        public static LoggerFactory GetFactory()
        {
            return new LoggerFactory();
        }

        public void IncreaseThreads()
        {
            _logger.Info("increaseThreads() called.");
            if (MAX_THREAD_AMOUNT <= senderThreads.Count)
                return;

            AddSenderThread();
            Thread thread = new Thread(new ThreadStart(AddSenderThread));
            senderThreads.Add(thread, _threadsender);

            thread.Start();
            _logger.Info("Added new sender thread to Aino.io logger core.");
        }



        public void AddMessage(AinoMessage msg)
        {
            _logger.Debug($"Adding message to the queue: {msg.ToJson()}");
            _messages.Enqueue(msg);
            _dataDelegates(_messages.Count);
        }


        // XX todo jono 
        // XX todo threadi_homma
        // XX todo http senderi
        // XX todo timeri jolla lähetetään
        // XX todo conffaus
        // XX todo checker message:n addin yhteyteen että pitääkö lähettää?
        // XX todo http sender error ja resend
        // todo logging. log4net nuget lisätty

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                }
            }
            //dispose unmanaged resources
            _logger.Info("Stopping sender thread.");
            _sender.Stop = true;
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Factory class for constructing {@link Agent} agent.
        /// </summary>
        public class LoggerFactory
        {
            private IAgentConfigBuilder builder;

            /**
             * Sets the configuration builder this factory should use.
             *
             * @param builder builder to use
             * @return this factory
             */
            public LoggerFactory SetConfigurationBuilder(IAgentConfigBuilder builder)
            {
                this.builder = builder;
                return this;
            }

            /// <summary>
            /// Builds the logger agent.
            /// </summary>
            /// <returns>configured logger agent</returns>
            public Agent Build()
            {
                AgentConfig agentConfig;

                if (null == builder)
                {
                    throw new InvalidAgentConfigException("No builder specified!");
                }
                else
                {
                    agentConfig = builder.Build();
                }

                return new Agent(agentConfig);
            }

        }
    }
}
