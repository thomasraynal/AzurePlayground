using Dasein.Core.Lite.Shared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace AzurePlayground.Authentication
{
    public class IdentityHandlerException : Exception, IHasHttpServiceError
    {
        private HttpServiceError _httpServiceError;

        public IdentityHandlerException()
        {
            CreateModel();
        }

        public IdentityHandlerException(string message) : base(message)
        {
            CreateModel();
        }

        public IdentityHandlerException(string message, Exception innerException) : base(message, innerException)
        {
            CreateModel();
        }

        protected IdentityHandlerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            CreateModel();
        }

        private void CreateModel()
        {
            var statusCode = this.Message.Contains("unauthorized") ? HttpStatusCode.Unauthorized : HttpStatusCode.BadRequest;
            _httpServiceError = new HttpServiceError()
            {
                HttpStatusCode = statusCode,
                ServiceErrorModel = new ServiceErrorModel()
                {
                    Reason = statusCode.ToString(),
                    Details = Message
                }
            };
        }

        public HttpServiceError HttpServiceError
        {
            get
            {
                return _httpServiceError;
            }
        }
    }
}
