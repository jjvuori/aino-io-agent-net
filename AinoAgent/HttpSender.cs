using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
        private readonly MessageQueue _queue;
        private readonly Configuration _configuration;
        private readonly AutoResetEvent _autoEvent;
        

        public HttpSender(MessageQueue queue, Configuration conf)
        {
            _autoEvent = new AutoResetEvent(false);
            _queue = queue;
            _configuration = conf;
        }

        internal void DataAdded(int size)
        {
            Debug.WriteLine("Data added called!");
            if (size >= _configuration.SizeThreshold)
            {
                Debug.WriteLine("Size threshold exceeded. Signaling.");
                _autoEvent.Set();
            }
        }

        internal void StartSending()
        {
            while (!Stop)
            {
                _autoEvent.WaitOne(_configuration.SendInterval);
                SendData();
            }

            Console.WriteLine("Stopping sender thread");
        }

        private async void SendData()
        {
            Debug.WriteLine("Sending called.");

            // TODO if sending fails, try to resend the same data?

            if (_queue.IsEmpty) return;
            
            using (var client = new HttpClient(new HttpClientHandler() {AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate}, true))
            {
                
                var jsonString = _queue.ToJson();

                StreamContent content = new StreamContent(GetDataStream(jsonString));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                if (_configuration.Gzip)
                {
                    content.Headers.ContentEncoding.Add("gzip");
                }
                
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("apikey", _configuration.ApiKey);

                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, _configuration.ApiAddress);
                req.Content = content;

                var response = await client.SendAsync(req);
                
                var responseStr = await response.Content.ReadAsStringAsync();

                Debug.WriteLine("Response: " + responseStr);
            }
        }

        private Stream GetDataStream(string jsonString)
        {

            if (_configuration.Gzip)
            {
                var ms = new MemoryStream();
                using (var gzip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    var data = Encoding.UTF8.GetBytes(jsonString);
                    gzip.Write(data, 0, data.Length);
                    gzip.Close(); 

                    ms.Position = 0;
                 
                    return ms;
                }
            }

            var dataStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));

            return dataStream;
        }
    }
}
