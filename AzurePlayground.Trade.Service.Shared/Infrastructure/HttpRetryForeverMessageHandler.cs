using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AzurePlayground.Service.Shared
{
    public class HttpRetryForeverMessageHandler : DelegatingHandler
    {
        private double _retryTimeout;

        public HttpRetryForeverMessageHandler(double retryTimeout)
        {
            _retryTimeout = retryTimeout;
            InnerHandler = new HttpClientHandler();
        }

        public HttpRetryForeverMessageHandler(HttpMessageHandler innerHandler, double retryTimeout) : base(innerHandler)
        {
            _retryTimeout = retryTimeout;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var result = Policy
                .Handle<HttpRequestException>()
                .Or<TaskCanceledException>()
                .OrResult<HttpResponseMessage>(x => !x.IsSuccessStatusCode)
                .WaitAndRetryForever(retryAttempt => TimeSpan.FromMilliseconds(_retryTimeout))
                .Execute(() => base.SendAsync(request, cancellationToken).Result);

            return Task.FromResult(result);
        }
    }
}
