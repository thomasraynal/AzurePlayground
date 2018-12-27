using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class UserProfile
    {
        public string Username { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
        public string Token { get; set; }
    }
}
