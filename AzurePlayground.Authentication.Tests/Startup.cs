using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AzurePlayground.Service.Shared;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
                options.DefaultChallengeScheme = "oidc";

            })
                .AddCookie("Cookies")
     
                    //.AddOpenIdConnect("oidc-hybrid", options =>
                    //{
                    //    options.Authority = "http://localhost:5001";
                    //    options.RequireHttpsMetadata = false;
                    //    options.Scope.Add("office");
                    //    options.ClientSecret = "secret";
                    //    options.ResponseType = "code id_token";
                    //    options.GetClaimsFromUserInfoEndpoint = true;
                    //    //"office_number", "office_number");
                    //    //options.Scope.Add(IdentityServerConstants.StandardScopes.Profile);
                    //    //options.Scope.Add(IdentityServerConstants.StandardScopes.OpenId);
                    //    //options.Scope.Add(AzurePlaygroundConstants.Api.Name);
                    //    options.ClientId = AzurePlaygroundConstants.Auth.ClientOpenIdHybrid;
                    //    options.SaveTokens = true;
                    //});
                    .AddOpenIdConnect("oidc", options =>
                    {
                        options.Authority = "http://localhost:5001";
                        options.RequireHttpsMetadata = false;
                        options.Scope.Add("office");
                        options.UseTokenLifetime = true;
                        options.GetClaimsFromUserInfoEndpoint = true;
                        //options.Scope.Add(IdentityServerConstants.StandardScopes.Profile);
                        //options.Scope.Add(IdentityServerConstants.StandardScopes.OpenId);
                        //options.Scope.Add(AzurePlaygroundConstants.Api.Name);
                        options.ClientId = AzurePlaygroundConstants.Auth.ClientOpenId;
                        options.SaveTokens = true;
                  
                    });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{

            //}

            app.UseExceptionHandler("/Home/Error");

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
