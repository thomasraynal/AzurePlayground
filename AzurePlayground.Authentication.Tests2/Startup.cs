using System.IdentityModel.Tokens.Jwt;
using AzurePlayground.Service.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AzurePlayground.Authentication.Tests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc-hybrid";
            })
                .AddCookie("Cookies")
                .AddOpenIdConnect("oidc-hybrid", options =>
                {
                    options.Authority = "http://localhost:5001";
                    options.RequireHttpsMetadata = false;
                    options.Scope.Add(AzurePlaygroundConstants.Desk.DeskScope);
                    options.ClientSecret = "secret";
                    options.ResponseType = "code id_token";
                    options.GetClaimsFromUserInfoEndpoint = true;
                    //"office_number", "office_number");
                    //options.Scope.Add(IdentityServerConstants.StandardScopes.Profile);
                    //options.Scope.Add(IdentityServerConstants.StandardScopes.OpenId);
                    //options.Scope.Add(AzurePlaygroundConstants.Api.Name);
                    options.ClientId = AzurePlaygroundConstants.Auth.ClientOpenIdHybrid2;
                    options.SaveTokens = true;
                });
                    //.AddOpenIdConnect("oidc", options =>
                    //{
                    //    options.Authority = "http://localhost:5001";
                    //    options.RequireHttpsMetadata = false;
                    //    options.Scope.Add("office");
                    //    //options.Scope.Add(IdentityServerConstants.StandardScopes.Profile);
                    //    //options.Scope.Add(IdentityServerConstants.StandardScopes.OpenId);
                    //    //options.Scope.Add(AzurePlaygroundConstants.Api.Name);
                    //    options.ClientId = AzurePlaygroundConstants.Auth.ClientOpenId;
                    //    options.SaveTokens = true;
                    //});
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
