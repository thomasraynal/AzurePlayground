using Polly;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public static class RetryPolicies
    {
        public static void WaitAndRetryForever<TException>(TimeSpan attemptDelay, Action<Exception, TimeSpan> onRetry, Action doTry) where TException : Exception
        {
            Policy
               .Handle<TException>()
               .WaitAndRetryForever(attempt => attemptDelay, onRetry)
               .Execute(doTry);
        }

        public static TResponse WaitAndRetryForever<TException, TResponse>(TimeSpan attemptDelay, Action<Exception, TimeSpan> onRetry, Func<TResponse> doTry) where TException : Exception
        {
            return Policy
                .Handle<TException>()
                .WaitAndRetryForever(attempt => attemptDelay, onRetry)
                .Execute(doTry);
        }
    }
}
