using Dasein.Core.Lite;
using Dasein.Core.Lite.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Dasein.Core.Lite.Hosting;
using FluentValidation.AspNetCore;
using System;
using Microsoft.AspNetCore.Identity;
using MongoDbGenericRepository;
using System.Linq;
using AzurePlayground.Persistence;
using AzurePlayground.Persistence.Mongo;
using IdentityServer4.Stores;
using AzurePlayground.Service.Shared;
using IdentityModel;
using IdentityServer4.Validation;

namespace AzurePlayground.Authentication
{
    public class AuthenticationServiceStartup : ServiceStartupBase<AuthenticationServiceConfiguration>
    {
        public AuthenticationServiceStartup(IHostingEnvironment env, IConfiguration configuration) : base(env, configuration)
        {
        }

        protected override void ConfigureServicesInternal(IServiceCollection services)
        {
            services.AddSerilog(Configuration);
            services.AddSingleton<ICacheStrategy<MethodCacheObject>, DefaultCacheStrategy<MethodCacheObject>>();

            var mongoDbContext = new MongoDbContext(ServiceConfiguration.MongoConnectionString, ServiceConfiguration.MongoDatabase);

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1;
                options.Password.RequiredUniqueChars = 1;
            })
                .AddMongoDbStores<IMongoDbContext>(mongoDbContext)
                .AddDefaultTokenProviders();

            services.AddIdentityServer(options =>
            {
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
            })
                .AddMongoStores()
                .AddProfileService<AllAvailableClaimsProfileService>()
                .AddSigningCredential(IdentityServerBuilderExtensionsCrypto.CreateRsaSecurityKey())
                .Services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
               


            services.AddAuthorization(options =>
            {
                options.AddPolicy(AzurePlaygroundConstants.Auth.AdminRolePolicy, policy => policy.RequireRole(AzurePlaygroundConstants.Auth.AdminRoleClaimValue));
            });

            services.AddSwagger(ServiceConfiguration);

            var jsonSettings = new TradeServiceJsonSerializerSettings();

            services.AddMvc()
                .RegisterJsonSettings(jsonSettings)
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblies((assembly) => assembly.FullName.Contains("AzurePlayground")));

        }

        protected override void ConfigureInternal(IApplicationBuilder app)
        {
            app.UseIdentityServer();

            app.SeedIdentityServer(
              clients: DbSeed.GetSeedClients(ServiceConfiguration), 
              apiRessources : DbSeed.GetSeedApiResources(ServiceConfiguration), 
              identityRessources : DbSeed.GetSeedIdentityRessources());

            app.SeedApplicationUser(DbSeed.GetSeedUsers());

            if (HostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseSwagger(ServiceConfiguration);

            app.UseMvcWithDefaultRoute();
        }
    }
}
