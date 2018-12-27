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
            public const string Client = "trade-service-auth-client";
        }

        public static class Desk
        {
            public const string DeskClaimType = "Desk";
            public const string DeskEquityClaim = "Desk-EQ";
            public const string DeskFixedIncomeClaim = "Desk-FIX";
        }

    }
}
