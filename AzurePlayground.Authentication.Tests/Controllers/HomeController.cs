using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AzurePlayground.Web.App.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using IdentityModel.Client;

namespace AzurePlayground.Web.App.Controllers
{
    public class HomeController : Controller
    {
        private IDiscoveryCache _discoveryCache;
        private IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory, IDiscoveryCache discoveryCache)
        {
            _discoveryCache = discoveryCache;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Trades()
        {
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var content = await client.GetStringAsync("http://localhost:5000/api/v1/trades");
                return Content(content);

            }
            catch (HttpRequestException ex)
            {
                var vm = new ErrorViewModel();
                var message =  ex.Message;
                return View("Error", vm);
            }
        }

        public IActionResult Logout()
        {
            return SignOut("Cookie", "oidc");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
