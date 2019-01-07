using Dasein.Core.Lite;
using Dasein.Core.Lite.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Dasein.Core.Lite.Hosting;
using FluentValidation.AspNetCore;
using System.IdentityModel.Tokens.Jwt;
using IdentityServer4.Validation;
using IdentityServer4.Stores;
using IdentityServer4;
using System;
using AzurePlayground.Persistence.Mongo;
using AzurePlayground.Persistence;

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

            services.AddIdentityServer(options =>
            {
                options.Events.RaiseSuccessEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
            })
                .AddMongoRepository()
                .AddSigningCredential(IdentityServerBuilderExtensionsCrypto.CreateRsaSecurityKey());


            services.AddAuthentication("ExpirationCookie")
                    .AddCookie("ExpirationCookie", options =>
                    {
                        options.ExpireTimeSpan = TimeSpan.FromSeconds(10);
                    });

            var mongoConfig = Configuration.Get<MongoConfigurationOptions>();
            services.AddIdentityMongoDbProvider<MongoUser>(options =>
            {
                options.MongoConnection = mongoConfig.MongoConnection;
                options.MongoDatabaseName = mongoConfig.MongoDatabaseName;
                options.RolesCollection = mongoConfig.RolesCollection;
                options.UsersCollection = mongoConfig.UsersCollection;
            });

            services.AddMongoDbConfiguration(Configuration);

            //services.AddSingleton<TestUserService>();
            services.AddTransient<IResourceOwnerPasswordValidator, TestResourceOwnerPasswordValidator>();
  
            var jsonSettings = new ServiceJsonSerializerSettings();

            this.AddSwagger(services);

            services.AddMvc()
                .RegisterJsonSettings(jsonSettings)
                .AddFluentValidation(config => config.RegisterValidatorsFromAssemblies((assembly) => assembly.FullName.Contains("AzurePlayground")));

        }

        protected override void ConfigureInternal(IApplicationBuilder app)
        {
            app.UseMongoDbForIdentityServer();

            app.UseIdentityServer();
            
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

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
