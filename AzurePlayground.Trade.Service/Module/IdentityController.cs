//using AzurePlayground.Service.Shared;
//using Dasein.Core.Lite;
//using Dasein.Core.Lite.Shared;
//using IdentityModel.Client;
//using IdentityServer4.Models;
//using IdentityServer4.Services;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Net.Http;
//using System.Threading.Tasks;

//namespace AzurePlayground.Service
//{
//    public class IdentityController : ServiceControllerBase
//    {
//        private UserManager<AzurePlaygroundUser> _userManager;
//        private SignInManager<AzurePlaygroundUser> _signInManager;
//        private IIdentityServerInteractionService _interaction;
//        private TradeServiceConfiguration _configuration;

//        public IdentityController(TradeServiceConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        private async Task<string> GetAccessToken()
//        {
//            var client = new HttpClient();

//            var request = new DiscoveryDocumentRequest()
//            {
//                Policy =
//                {
//                    RequireHttps = false,
//                    RequireKeySet = false,
//                    ValidateEndpoints = false,
//                    ValidateIssuerName = false,
//                },
//                Address = "http://localhost:5001/.well-known/openid-configuration"
//            };

//            var disco = await client.GetDiscoveryDocumentAsync(request);

//            if (disco.IsError) throw new Exception(disco.Error);

//            var accessToken = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
//            {
//                Address = "http://localhost:5001/connect/token",

//                ClientId = "AzurePlaygroundUserClient",
//                ClientSecret = _configuration.Key.Sha256(),
//                Scope = "AzurePlayground.Trade.Service",
//                UserName = "alice",
//                Password = "password"
//            });


//            if (accessToken.IsError)
//            {
//                Console.WriteLine(accessToken.Error);
//                return accessToken.Error;
//            }

//            Console.WriteLine(accessToken.Json);

//            return accessToken.AccessToken;
//        }

//        [HttpPost]
//        public async Task<IActionResult> Index([FromBody] Credentials credentials)
//        {
//            // //_userManager.FindByNameAsync()

//            // var dic = new Dictionary<String, string>
//            // {
//            //     ["Username"] = credentials.Username,
//            //     ["Password"] = credentials.Password
//            // };

//            // return Challenge(new AuthenticationProperties(dic), IdentityServerAuthenticationDefaults.AuthenticationScheme);
//            ////var result = await _signInManager.PasswordSignInAsync(credentials.Username, credentials.Password, true, false);
//            //// return Ok();

//            using (var client = new HttpClient())
//            {
//                var accessToken = await GetAccessToken();

//                client.SetBearerToken(accessToken);

//                var response = await client.GetAsync("http://localhost:5001/api/ApiResourceWithPolicy");

//                if (!response.IsSuccessStatusCode)
//                {
//                    return BadRequest();
//                }

//                var content = await response.Content.ReadAsStringAsync();

//                return Ok();
//            }

//        }
//    }
//}
