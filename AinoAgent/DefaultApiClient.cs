using Aino.Agents.Core.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;

namespace Aino.Agents.Core
{
    class DefaultApiClient : IApiClient
    {
        //private static readonly string AUTHORIZATION_HEADER = "Authorization";

        private readonly AgentConfig agentConfig;
        //private final WebResource resource;
        private HttpClient _client;

        public DefaultApiClient(AgentConfig config)
        {
            this.agentConfig = config;
            HttpMessageHandler connection = HttpProxyFactory.GetConnectionHandler(agentConfig);
            HttpClient restClient = new HttpClient(connection);
            restClient.BaseAddress = new Uri(agentConfig.GetLogServiceUri());
        }

        public IApiResponse Send(byte[] data)
        {
            HttpResponseMessage response = _client.SendAsync(BuildRequest()).Result;
            return new ApiResponseImpl(response);
        }

        private HttpRequestMessage BuildRequest()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, agentConfig.GetLogServiceUri());
            if (agentConfig.IsGzipEnabled())
            {
                _client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                _client.DefaultRequestHeaders.TransferEncoding.Add(new TransferCodingHeaderValue("gzip"));
            }

            return request;
        }

        class ApiResponseImpl : IApiResponse
        {
            private readonly HttpResponseMessage response;

            public ApiResponseImpl(HttpResponseMessage response)
            {
                this.response = response;
            }

            public int GetStatus()
            {
                return (int)response.StatusCode;
            }

            public string GetPayload()
            {
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
