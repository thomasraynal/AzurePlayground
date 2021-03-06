﻿using AspNetCore.Identity.MongoDbCore.Models;
using AzurePlayground.Persistence;
using AzurePlayground.Persistence.Mongo;
using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace AzurePlayground.Authentication
{
    public static class DbSeed
    {
        public static IEnumerable<PasswordHolder<ApplicationUser>> GetSeedUsers()
        {
            return new List<PasswordHolder<ApplicationUser>>()
            {
                new PasswordHolder<ApplicationUser>(new ApplicationUser("alice.smith"){
                    Claims =
                    {
                        new MongoClaim(){Type =JwtClaimTypes.Subject, Value =Guid.NewGuid().ToString() },
                        new MongoClaim(){Type =JwtClaimTypes.Email, Value = "alice.smith@bfi.com" },
                        new MongoClaim(){Type =JwtClaimTypes.Name, Value = "Alice Smith" },
                        new MongoClaim(){Type =JwtClaimTypes.GivenName, Value = "Alice" },
                        new MongoClaim(){Type =JwtClaimTypes.FamilyName, Value = "Smith" },
                        new MongoClaim(){Type =ClaimTypes.Role, Value = AzurePlaygroundConstants.Auth.AdminRoleClaimValue},
                        new MongoClaim(){Type =AzurePlaygroundConstants.Desk.DeskClaimType, Value =  AzurePlaygroundConstants.Desk.DeskEquityClaim}
                    }
                }, "alice"),
                 new PasswordHolder<ApplicationUser>(new ApplicationUser("bob.woodworth"){
                    Claims =
                    {
                        new MongoClaim(){Type =JwtClaimTypes.Subject, Value =Guid.NewGuid().ToString() },
                         new MongoClaim(){Type =JwtClaimTypes.Email, Value = "bob.woodworth@bfi.com" },
                        new MongoClaim(){Type =JwtClaimTypes.Name, Value = "Bob Woodworth" },
                        new MongoClaim(){Type =JwtClaimTypes.GivenName, Value = "Bob" },
                        new MongoClaim(){Type =JwtClaimTypes.FamilyName, Value = "Woodworth" },
                        new MongoClaim(){Type =AzurePlaygroundConstants.Desk.DeskClaimType, Value =  AzurePlaygroundConstants.Desk.DeskFixedIncomeClaim}
                    }
                }, "bob"),
                 new PasswordHolder<ApplicationUser>(new ApplicationUser("internal"){
                    Claims =
                    {
                        new MongoClaim(){Type =JwtClaimTypes.Subject, Value =Guid.NewGuid().ToString() },
                    }
                }, "idkfa")
            };
        }

        public static IEnumerable<ApiResource> GetSeedApiResources(IServiceConfiguration configuration)
        {
            return new List<ApiResource>()
            {
                new ApiResource(AzurePlaygroundConstants.Api.Trade, AzurePlaygroundConstants.Api.Description)
                {
                    ApiSecrets = { new Secret(configuration.Key.Sha256()) }                    
                }
            };
        }
            
        public static IEnumerable<IdentityResource> GetSeedIdentityRessources()
        {
            return new List<IdentityResource>()
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                AzurePlaygroundConstants.Desk.DeskIdentityResource
            };
        }

        public static IEnumerable<Client> GetSeedClients(IServiceConfiguration configuration)
        {
            return new List<Client>
            {
                //reference token client
                new Client
                {
                    ClientId = AzurePlaygroundConstants.Auth.ClientReferenceToken,
                    ClientSecrets = { new Secret { Value = configuration.Key.Sha256()},
                    },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = { AzurePlaygroundConstants.Api.Trade },
                    AccessTokenLifetime = 3600 * 24
                },

                //oidc client
                //new Client
                //{
                //    ClientId =  AzurePlaygroundConstants.Auth.ClientOpenId,
                //    ClientName =  AzurePlaygroundConstants.Auth.ClientOpenId,
                //    AllowedGrantTypes = GrantTypes.Implicit,
                //    RequireConsent = false,
                //    AccessTokenType = AccessTokenType.Reference,
                //    //AccessTokenLifetime =5,
                //    //AuthorizationCodeLifetime =5,
                //    //IdentityTokenLifetime = 5,
                //    RedirectUris = { "http://localhost:5002/signin-oidc" },
                //    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },
                //    AllowedScopes = { AzurePlaygroundConstants.Api.Name,IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, AzurePlaygroundConstants.Desk.DeskScope},
                //},
                 //oidc client hybrid
                new Client
                {
                    ClientId =  AzurePlaygroundConstants.Auth.ClientOpenIdHybrid,
                    ClientName =  AzurePlaygroundConstants.Auth.ClientOpenIdHybrid,
                    AccessTokenType = AccessTokenType.Reference,
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowOfflineAccess = true,
                    //AccessTokenLifetime =30,
                    ClientSecrets = { new Secret(configuration.Key.Sha256()) },
                    RequireConsent = false,
                    RedirectUris = { $"http://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },
                    FrontChannelLogoutUri =  "http://localhost:5002/signout-callback-oidc",
                    AllowedScopes = { AzurePlaygroundConstants.Api.Trade,IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, AzurePlaygroundConstants.Desk.DeskScope},
                },
                //pkce client
                 new Client
                {
                    ClientId =  AzurePlaygroundConstants.Auth.ClientOpenIdNative,
                    ClientName =  AzurePlaygroundConstants.Auth.ClientOpenIdNative,
                    RedirectUris = { "http://127.0.0.1/sample-wpf-app" },
                    PostLogoutRedirectUris = { "http://127.0.0.1/sample-wpf-app" },
                    RequireClientSecret = false,
                    RequireConsent = false,
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    AllowedScopes = {  AzurePlaygroundConstants.Api.Trade,IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile,AzurePlaygroundConstants.Desk.DeskScope },
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse
                }
            };
        }
    }
}
