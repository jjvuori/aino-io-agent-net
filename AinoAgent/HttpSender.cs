using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
        private bool _retry = false;
        private int _retryCount = 0;
        private byte[] _lastData;
        

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

        internal void  StartSending()
        {
            while (!Stop)
            {
                _autoEvent.WaitOne(_configuration.SendInterval);
                SendData();
            }

            Console.WriteLine("Stopping sender thread");
        }

        private void SendData()
        {
            Debug.WriteLine("Sending called.");

            // TODO if sending fails, try to resend the same data?

            if (_queue.IsEmpty) return;

            using (var client = new HttpClient( new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }, true))
            using (var data = new MemoryStream())
            {

                StreamContent content;
                if (_retry)
                {
                    content = new StreamContent(GetDataStream(_lastData));
                }
                else
                {
                    _queue.ToJson(data);
                    _lastData = data.ToArray();
                    ModifyDataStream(data);
                    content = new StreamContent(data);
                }
                

                SetHeaders(content, client);

                var req = new HttpRequestMessage(HttpMethod.Post, _configuration.ApiAddress);
                req.Content = content;

                var response = client.SendAsync(req);

                var responseStr = response.Result;

                HandleResponse(response.Result);

                Debug.WriteLine("Response: " + responseStr);
            }
        }

        private void HandleResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                _retry = false;
                _retryCount = 0;
                return;
            }

            if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.InternalServerError)
            {
                // Aino io blew up?! Let's not try to resend these.
                _retry = false;
                _retryCount = 0;
                return;
            }

            if (_retryCount > 5)
            {
                _retry = false;
                _retryCount = 0;
                // TODO log nasty errors!
            }
            else
            {
                _retry = true;
                _retryCount++;
            }
        }

        private void SetHeaders(HttpContent content, HttpClient client)
        {
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("apikey", _configuration.ApiKey);

            if (_configuration.Gzip)
            {
                content.Headers.ContentEncoding.Add("gzip");
            }
        }

        private void ModifyDataStream(MemoryStream jsonData)
        {
            if (!_configuration.Gzip) return;

            byte[] data = jsonData.ToArray();
            jsonData.SetLength(0);
            
            using (var gzip = new GZipStream(jsonData, CompressionMode.Compress, true))
            {
                gzip.Write(data, 0, data.Length);
                gzip.Close();

                jsonData.Position = 0;
            }
        }

        private Stream GetDataStream(byte[] jsonData)
        {

            if (_configuration.Gzip)
            {
                var ms = new MemoryStream();
                using (var gzip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    
                    gzip.Write(jsonData, 0, jsonData.Length);
                    gzip.Close(); 

                    ms.Position = 0;
                 
                    return ms;
                }
            }

            var dataStream = new MemoryStream(jsonData);

            return dataStream;
        }
    }
}
