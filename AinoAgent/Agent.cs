using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Aino
{

    public class Agent : IDisposable
    {
        private readonly Thread _senderThread;
        private readonly HttpSender _sender;
        private readonly ConcurrentQueue<AinoMessage> _messages;
        

        public Agent()
        {
            _messages = new ConcurrentQueue<AinoMessage>();
            _sender = new HttpSender(_messages);
            _senderThread = new Thread(_sender.StartSending);
            _senderThread.Start();
        }

        public void AddMessage(AinoMessage msg)
        {
            _messages.Enqueue(msg);
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
