using System;
using System.Net;
using System.Net.Http;
using Google.Apis.Http;

namespace ZTask.Model.Core.Cloud
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
