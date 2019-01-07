using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzurePlayground.Service.Shared
{
    public class UserDescriptor
    {
        public UserDescriptor(string name, string password)
        {
            Name = name;
            Password = password;

        }

        public string Name { get; internal set; }
        public string Password { get; internal set; }
    }

    public static class AzurePlaygroundConstants
    {

        public static class Api
        {
            public const string Name = "trade";
            public const string Description = "Trade Service";
        }

        public static class Auth
        {
            public const string ClientReferenceToken = "trade-service-auth-client-reference-token";
            public const string ClientOpenId = "trade-service-auth-client-openid";
            public const string ClientOpenIdHybrid = "trade-service-auth-client-hybrid";
            public const string ClientOpenIdHybrid2 = "trade-service-auth-client-hybrid2";

        }

        public static class Desk
        {
            public static IdentityResource DeskIdentityResource { get; private set; }
            public static String DeskNameClaim = "desk_name";

            static Desk()
            {
                DeskIdentityResource = new IdentityResource() { Name = "desk", DisplayName = "Trader Desk Info", UserClaims = { DeskNameClaim } };
            }

            public const string DeskClaimType = "Desk";
            public const string DeskEquityClaim = "Desk-EQ";
            public const string DeskFixedIncomeClaim = "Desk-FIX";
        }

    }
}
