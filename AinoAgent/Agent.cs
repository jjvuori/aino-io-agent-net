using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Aino
{

    delegate void DataAdded(int size);

    public class Agent : IDisposable
    {
        private Thread _senderThread;
        private HttpSender _sender;
        private readonly MessageQueue _messages;
        private DataAdded _dataDelegates;
    

        public Configuration Configuration { get; set; }
        

        public Agent()
        {
            _messages = new MessageQueue();
        }


        public void Initialize()
        {
            if (Configuration == null)
            {
                throw new AinoException("Could not initialize. Configuration missing.");
            }

            _sender = new HttpSender(_messages, Configuration);
            _dataDelegates += _sender.DataAdded;
            _senderThread = new Thread(_sender.StartSending);
            _senderThread.Start();
        }

        public void AddMessage(AinoMessage msg)
        {
            _messages.Enqueue(msg);
            _dataDelegates(_messages.Count);
        }


        // XX todo jono 
        // XX todo threadi_homma
        // todo http senderi
        // todo timeri jolla lähetetään
        // todo conffaus
        // todo checker message:n addin yhteyteen että pitääkö lähettää?


        public void Dispose()
        {
            _sender.Stop = true; 
        }
    }
}
