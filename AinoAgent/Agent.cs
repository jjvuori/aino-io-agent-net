using System;
using System.Threading;
using log4net;


namespace Aino
{

    delegate void DataAdded(int size);

    public class Agent : IDisposable
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(Agent));

        private Thread _senderThread;
        private HttpSender _sender;
        private readonly MessageQueue _messages;
        private DataAdded _dataDelegates;
    

        public Configuration Configuration { get; set; }
        

        public Agent()
        {
            _logger.Info("Aino Agent created");
            _messages = new MessageQueue();
        }

        public Agent(Configuration configuration) : this()
        {
            _logger.Debug("Setting configuration object for agent");
            Configuration = configuration;
        }

        public void Initialize()
        {
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


        public void Dispose()
        {
            _logger.Info("Stopping sender thread.");
            _sender.Stop = true; 
        }
    }
}
