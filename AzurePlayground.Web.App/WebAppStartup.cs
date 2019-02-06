using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AzurePlayground.Web.App.Extensions;
using AzurePlayground.Service.Shared;
using Dasein.Core.Lite.Shared;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Net.Http.Headers;
using Dasein.Core.Lite;
using AzurePlayground.Web.App.Infrastructure;
using Newtonsoft.Json;

namespace AzurePlayground.Web.App
{
    public class WebAppStartup : ServiceStartupBase<AppConfiguration>
    {
        public WebAppStartup(IHostingEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
        }

        protected override void ConfigureServicesInternal(IServiceCollection services)
        {
            services.AddSerilog(Configuration);
            services.AddSingleton<ICacheStrategy<MethodCacheObject>, DefaultCacheStrategy<MethodCacheObject>>();

            var jsonSettings = new TradeServiceJsonSerializer();

            services.AddMvc()
                    .RegisterJsonSettings(jsonSettings);

            services.AddSingleton<IDiscoveryCache>(r =>
            {
                var factory = r.GetRequiredService<IHttpClientFactory>();
                return new DiscoveryCache(ServiceConfiguration.Identity, () => factory.CreateClient());
            });
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AzurePlaygroundConstants.Auth.AdminRolePolicy, policy => policy.RequireRole(AzurePlaygroundConstants.Auth.AdminRoleClaimValue));
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookie";
                options.DefaultChallengeScheme = "oidc";

            })
            .AddAutomaticTokenRefresh()
            .AddCookie("Cookie", options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromSeconds(5);
            })

            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = ServiceConfiguration.Identity;
                options.RequireHttpsMetadata = false;
                options.SignInScheme = "Cookie";
                options.Scope.Add(AzurePlaygroundConstants.Desk.DeskScope);
                options.ClientSecret = "AQMZwz4588oyWcIxdDDLf";
                options.ResponseType = "code id_token";

                options.UseTokenLifetime = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.Scope.Clear();
                options.Scope.Add(IdentityServerConstants.StandardScopes.Profile);
                options.Scope.Add(IdentityServerConstants.StandardScopes.OpenId);
                options.Scope.Add(AzurePlaygroundConstants.Api.Trade);
                options.Scope.Add("offline_access");

                options.ClaimActions.MapAll();
                options.ClientId = AzurePlaygroundConstants.Auth.ClientOpenIdHybrid;
                options.SaveTokens = true;

            });

        }

        protected override void ConfigureInternal(IApplicationBuilder app)
        {

            app.UseExceptionHandler("/Home/Error");

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
