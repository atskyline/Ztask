using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Http;

namespace TestGoogleApi
{
    class ProxyableHttpClientFactory : HttpClientFactory
    {
        public String ProxyAddress { get; private set;}

        public ProxyableHttpClientFactory(String proxyAddress)
        {
            this.ProxyAddress = proxyAddress;
        }

        protected override HttpMessageHandler CreateHandler(CreateHttpClientArgs args)
        {
            var handler =  (HttpClientHandler)base.CreateHandler(args);
            handler.Proxy = new WebProxy(this.ProxyAddress);
            return handler;
        }
    }
}
