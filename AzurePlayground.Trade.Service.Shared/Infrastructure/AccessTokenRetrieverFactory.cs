using Dasein.Core.Lite.Shared;
using IdentityModel.Client;
using Polly;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;

namespace AzurePlayground.Service.Shared
{
    public class AccessTokenRetrieverFactory : ICanLog
    {
        private TokenResponse _response;
        private JwtSecurityTokenHandler _jwthandler;

        public AccessTokenRetrieverFactory()
        {
            _jwthandler = new JwtSecurityTokenHandler();
        }

        public Func<string> GetToken(
            string username,
            string password,
            string authorityUrl,
            string clientName,
            string apiName,
            string apiSecret)
        {

            return () =>
            {

                if (null != _response)
                {
                    var jwttoken = _jwthandler.ReadToken(_response.AccessToken);

                    if (jwttoken.ValidTo > DateTime.Now)
                    {
                        return _response.AccessToken;
                    }
                }

                var client = new HttpClient();

                var request = new DiscoveryDocumentRequest()
                {
                    Policy ={
                        RequireHttps = false,
                        RequireKeySet = false,
                        ValidateEndpoints = false,
                        ValidateIssuerName = false },
                    Address = authorityUrl
                };

                return RetryPolicies.WaitAndRetryForever<Exception, string>(
                     attemptDelay: TimeSpan.FromMilliseconds(10000),
                     onRetry: (ex, timespan) => this.LogError($"Failed to reach auth server {authorityUrl} - [{ex}]- [{ex.InnerException}]"),
                     doTry: () =>
                     {
                         var disco = client.GetDiscoveryDocumentAsync(request).Result;

                         if (disco.IsError) throw new Exception(disco.Error);

                         _response = client.RequestPasswordTokenAsync(new PasswordTokenRequest
                         {
                             Address = disco.TokenEndpoint,
                             ClientId = clientName,
                             ClientSecret = apiSecret,
                             Scope = apiName,
                             UserName = username,
                             Password = password
                         }).Result;


                         if (_response.IsError)
                         {
                             throw new Exception("Failed to acquire token", _response.Exception);
                         }

                         return _response.AccessToken;
                     }
                     );

            };
        }
    }
}
