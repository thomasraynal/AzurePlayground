using IdentityModel;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace AzurePlayground.Authentication
{
    public class TradeServiceUser
    {
        public string SubjectId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ProviderName { get; set; }
        public string ProviderSubjectId { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Claim> Claims { get; set; } = new HashSet<Claim>(new ClaimComparer());
    }
}
