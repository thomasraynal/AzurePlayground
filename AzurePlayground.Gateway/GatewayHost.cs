using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;

namespace AzurePlayground.Gateway
{
    public class GatewayHost
    {
        public const String serviceConfigFile = "config.json";
        public const String ocelotConfigFile = "ocelot.json";

        protected virtual IConfigurationBuilder Configure(IConfigurationBuilder builder)
        {
            return builder;
        }

        public IWebHost Build(params string[] args)
        {

            var builder = new ConfigurationBuilder()
                .AddJsonFile(serviceConfigFile, true, true)
                .AddJsonFile(ocelotConfigFile, true, true)
                .AddCommandLine(args)
                .AddEnvironmentVariables();

            builder = Configure(builder);

            var config = builder.Build();

            return new WebHostBuilder()
                .UseConfiguration(config)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<GatewayServiceStartup>()
                .UseKestrel()
                //.UseSerilog((builderContext, configuration) =>
                //{
                //    configuration
                //        .MinimumLevel.Information()
                //        .Enrich.FromLogContext()
                //        .WriteTo.Console();
                //})
                .Build();
        }
    }
}
