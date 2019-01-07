using Dasein.Core.Lite.Shared;
using IdentityModel;
using IdentityServer4.Test;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AzurePlayground.Authentication
{
    public class TradeServiceUserStore
    {
        public List<TradeServiceUser> Users { get; }

        public TradeServiceUserStore()
        {
            Users = DbSeed.GetSeedUsers().ToList();
        }
    }
}
