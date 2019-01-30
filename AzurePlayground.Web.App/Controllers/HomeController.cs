using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AzurePlayground.Web.App.Models;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using IdentityModel.Client;
using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;

namespace AzurePlayground.Web.App
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
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
