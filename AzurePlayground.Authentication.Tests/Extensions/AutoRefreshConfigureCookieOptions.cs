using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzurePlayground.Web.App.Extensions
{
    public class AutoRefreshConfigureCookieOptions : IConfigureNamedOptions<CookieAuthenticationOptions>
    {
        private readonly AuthenticationScheme _signInScheme;

        public AutoRefreshConfigureCookieOptions(IAuthenticationSchemeProvider provider)
        {
            _signInScheme = provider.GetDefaultSignInSchemeAsync().GetAwaiter().GetResult();
        }

        public void Configure(CookieAuthenticationOptions options)
        { }

        public void Configure(string name, CookieAuthenticationOptions options)
        {
            if (name == _signInScheme.Name)
            {
                options.EventsType = typeof(AutoRefreshCookieEvents);
            }
        }
    }
}
