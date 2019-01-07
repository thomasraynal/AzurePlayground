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
        public static IEnumerable<TradeServiceUser> GetSeedUsers()
        {
            return new List<TradeServiceUser>()
        {
            new TradeServiceUser{SubjectId = "818727", Username = "alice.smith", Password = "alice",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Alice Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Alice"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.Email, "AliceSmith@bfi.com"),
                   new Claim(AzurePlaygroundConstants.Desk.DeskClaimType, AzurePlaygroundConstants.Desk.DeskEquityClaim),


                }
            },
            new TradeServiceUser{SubjectId = "88421113", Username = "bob.woodworth", Password = "bob",
                Claims =
                {
                    new Claim(JwtClaimTypes.Name, "Bob Woodworth"),
                    new Claim(JwtClaimTypes.GivenName, "Bob"),
                    new Claim(JwtClaimTypes.FamilyName, "Woodworth"),
                    new Claim(JwtClaimTypes.Email, "BobWoodworth@bfi.com"),
                   new Claim(AzurePlaygroundConstants.Desk.DeskClaimType, AzurePlaygroundConstants.Desk.DeskEquityClaim)
                }
            }
        };
        }

        public static IEnumerable<ApiResource> GetSeedApiResources(IServiceConfiguration configuration)
        {
            return new List<ApiResource>()
            {
                new ApiResource(AzurePlaygroundConstants.Api.Name, AzurePlaygroundConstants.Api.Description)
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
                    ClientSecrets = {

                        new Secret{
                            Type = IdentityServerConstants.SecretTypes.SharedSecret,
                            Value = configuration.Key.Sha256()
                        },
                        },

                    AccessTokenType = AccessTokenType.Reference,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes = { AzurePlaygroundConstants.Api.Name, IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile},
                    AccessTokenLifetime = 30
                },
                //oidc client
                new Client
                {
                    ClientId =  AzurePlaygroundConstants.Auth.ClientOpenId,
                    ClientName =  AzurePlaygroundConstants.Auth.ClientOpenId,
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RequireConsent = false,
                    AccessTokenLifetime =5,
                    AuthorizationCodeLifetime =5,
                    IdentityTokenLifetime = 5,
                    RedirectUris = { "http://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },
                    AllowedScopes = { AzurePlaygroundConstants.Api.Name,IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, "office"},
                },
                 //oidc client hybrid
                new Client
                {
                    ClientId =  AzurePlaygroundConstants.Auth.ClientOpenIdHybrid,
                    ClientName =  AzurePlaygroundConstants.Auth.ClientOpenIdHybrid,
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowOfflineAccess = true,
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    RequireConsent = false,
                    RedirectUris = { "http://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },
                    FrontChannelLogoutUri =  "http://localhost:5002/signout-callback-oidc",
                    AllowedScopes = { AzurePlaygroundConstants.Api.Name,IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, "office"},
                },
                //pkce client
                 new Client
                {
                    ClientId = "native.code",
                    ClientName = "Native Client (Code with PKCE)",
                    RedirectUris = { "http://127.0.0.1/sample-wpf-app" },
                    PostLogoutRedirectUris = { "http://127.0.0.1/sample-wpf-app" },
                    RequireClientSecret = false,
                    RequireConsent = false,
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    AllowedScopes = {  AzurePlaygroundConstants.Api.Name,IdentityServerConstants.StandardScopes.OpenId, IdentityServerConstants.StandardScopes.Profile, "office" },
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse
                }
            };
        }
    }
}
