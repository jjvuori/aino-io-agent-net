using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading;

namespace Aino
{
    internal class HttpSender
    {
        internal volatile bool Stop = false;
        private ConcurrentQueue<AinoMessage> _queue;

        // TODO as conf!
        //private const string AINO_API_ADDRESS = "http://localhost:9090/api/v1";
        private const string AINO_API_ADDRESS = "https://data.aino.io/rest/v2.0/transaction";
        private const string AINO_API_KEY = "67437716-ed3c-4f6b-84eb-a6b5dd560f3c";


        public HttpSender(ConcurrentQueue<AinoMessage> queue)
        {
            _queue = queue;
        }


        internal void StartSending()
        {
            while (!Stop)
            {
                Thread.Sleep(1000);
                Console.WriteLine("Looping! Whee!");
                SendData();
            }

            Console.WriteLine("Stopping sender thread");
        }

        private async void SendData()
        {
            if (_queue.IsEmpty) return;

            using (var client = new HttpClient())
            {
                AinoMessage msg;
                _queue.TryDequeue(out msg);
                if (msg == null) return;

                StreamContent content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("{\"transactions\": [" + msg.ToJson() + "]}")));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("apikey", AINO_API_KEY);
                
                var response = await client.PostAsync(AINO_API_ADDRESS, content);

                var responseStr = await response.Content.ReadAsStringAsync();

                Debug.WriteLine("Response: " + responseStr);
            }
        }
    }
}
