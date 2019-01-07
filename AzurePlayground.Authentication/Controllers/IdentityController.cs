//using AzurePlayground.Service.Shared;
//using Dasein.Core.Lite;
//using IdentityModel.Client;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Security.Claims;
//using System.Threading.Tasks;

//namespace AzurePlayground.Authentication
//{
//    public class IdentityController : ServiceControllerBase, IIdentityService
//    {
//        private DiscoveryCache _cache;
//        private AuthenticationServiceConfiguration _configuration;

//        public IdentityController(AuthenticationServiceConfiguration configuration)
//        {
//            _cache = new DiscoveryCache(configuration.Identity, policy: new DiscoveryPolicy()
//            {
//                RequireHttps = false
//            });

//            _configuration = configuration;
//        }

//        private async Task<DiscoveryResponse> GetDiscoveryResponse()
//        {
//            var disco = await _cache.GetAsync();
//            if (disco.IsError) throw new IdentityHandlerException(disco.Error);
//            return disco;
//        }

//        private async Task<string> GetAccessToken(LoginRequest request)
//        {

//            var disco = await GetDiscoveryResponse();

//            using (var client = new HttpClient())
//            {

//                var accessToken = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
//                {
//                    Address = disco.TokenEndpoint,
//                    ClientId = AzurePlaygroundConstants.Auth.ClientReferenceToken,
//                    ClientSecret = _configuration.Key,
//                    Scope = $"{AzurePlaygroundConstants.Api.Name} openid",
//                    UserName = request.Username,
//                    Password = request.Password
//                });

//                if (accessToken.IsError)
//                {
//                    throw new IdentityHandlerException(accessToken.Error);
//                }

//                return accessToken.AccessToken;

//            }
//        }

//        private async Task<IEnumerable<Claim>> GetUserClaimsInternal(String token)
//        {

//            var disco = await GetDiscoveryResponse();

//            using (var client = new HttpClient())
//            {

//                if (disco.IsError) throw new IdentityHandlerException(disco.Error);

//                var userInfo = await client.GetUserInfoAsync(new UserInfoRequest()
//                {
//                    Address = disco.UserInfoEndpoint,
//                    Token = token
//                });

//                if (userInfo.IsError)
//                {
//                    throw new IdentityHandlerException(userInfo.Error);
//                }

//                return userInfo.Claims;

//            }
//        }

//        private async Task RevokeInternal(string token)
//        {

//            var disco = await GetDiscoveryResponse();

//            using (var client = new HttpClient())
//            {
//                if (disco.IsError) throw new Exception(disco.Error);

//                var revokedToken = await client.RevokeTokenAsync(new TokenRevocationRequest()
//                {
//                    ClientId = AzurePlaygroundConstants.Auth.ClientReferenceToken,
//                    ClientSecret = _configuration.Key,
//                    Address = disco.RevocationEndpoint,
//                    Token = token
//                });

//                if (revokedToken.IsError)
//                {
//                    throw new IdentityHandlerException(revokedToken.Error);
//                }

//            }
//        }

//        [Authorize]
//        [HttpPost("user")]
//        public async Task<IEnumerable<Claim>> GetUser([FromHeader(Name = "Authorization")] string token)
//        {
//            var claims = await GetUserClaimsInternal(token.Replace($"{JwtBearerDefaults.AuthenticationScheme} ", string.Empty));
//            return claims;
//        }

//        [Authorize]
//        [HttpPost("revoke")]
//        public async Task Revoke([FromBody] RevokeTokenRequest request)
//        {
//            await RevokeInternal(request.Token);
//        }

//        [Authorize]
//        [HttpPost("logout")]
//        public async Task Logout([FromHeader(Name = "Authorization")] string token)
//        {
//            await RevokeInternal(token);
//        }

//        [AllowAnonymous]
//        [HttpPost("login")]
//        public async Task<UserProfile> Login([FromBody] LoginRequest request)
//        {
//            var accessToken = await GetAccessToken(request);
//            var claims = await GetUserClaimsInternal(accessToken);

//            var profile = new UserProfile()
//            {
//                Username = request.Username,
//                Token = accessToken,
//                Claims = claims
//            };

//            return profile;
//        }
//    }
//}
