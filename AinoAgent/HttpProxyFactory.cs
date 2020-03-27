using Aino.Agents.Core.Config;
using System;
using System.Net;
using System.Net.Http;

namespace Aino.Agents.Core
{
    class HttpProxyFactory
    {
        //HttpWebRequest httpWebRequest; // = (HttpWebRequest)WebRequest.Create("");
        IWebProxy proxy;
        private HttpProxyFactory(IWebProxy proxy)
        {
            this.proxy = proxy;
        }

        public HttpWebRequest GetHttpURLConnection(Uri uri)
        {
            //Todo: Check if this connection needs to be separately opened.
            HttpWebRequest h = (HttpWebRequest)HttpWebRequest.Create(uri);
            h.Proxy = this.proxy;
            return h;
        }

        public static HttpClientHandler GetConnectionHandler(AgentConfig agentConfig)
        {
            HttpClientHandler httpclienthandler = new HttpClientHandler();
            if (agentConfig.IsProxyDefined())
            {
                WebProxy webProxy = new WebProxy(agentConfig.GetProxyHost(), agentConfig.GetProxyPort());
                //webProxy.BypassProxyOnLocal = false; //Todo: Is this needed?
                httpclienthandler.Proxy = webProxy;
            }

            return httpclienthandler;
        }
    }
}
