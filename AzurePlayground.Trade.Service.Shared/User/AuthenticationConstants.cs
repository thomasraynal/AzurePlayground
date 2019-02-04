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
        public static class Urls
        {
            public static string AppUrl = "http://app";
        }

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
            public const string ClientOpenIdNative = "native";

            public const string AdminRoleClaimValue = "Admin";
            public const string AdminRolePolicy = "AdminPolicy";
        }

        public static class Desk
        {
            public static IdentityResource DeskIdentityResource { get; private set; }
            public static String DeskClaimType = "desk_name";
            public static String DeskScope = "desk";

            static Desk()
            {
                DeskIdentityResource = new IdentityResource() { Name = DeskScope, DisplayName = "Trader Desk Info", UserClaims = { DeskClaimType } };
            }

      
            public const string DeskEquityClaim = "Desk-EQ";
            public const string DeskFixedIncomeClaim = "Desk-FIX";
        }

    }
}
