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

            services.AddAuthentication("authCookie")
                    .AddCookie("authCookie", options =>
                    {
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    });

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
                .AddSigningCredential(IdentityServerBuilderExtensionsCrypto.CreateRsaSecurityKey());
            
            this.AddSwagger(services);

            var jsonSettings = new ServiceJsonSerializerSettings();

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

            app.UseStaticFiles();

            this.UseSwagger(app);

            app.UseMvcWithDefaultRoute();
        }
    }
}
