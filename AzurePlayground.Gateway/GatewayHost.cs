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
        public const String ocelotConsulConfigFile = "ocelot.consul.json";

        protected virtual IConfigurationBuilder Configure(IConfigurationBuilder builder)
        {
            return builder;
        }

        public IWebHost Build(params string[] args)
        {

            var builder = new ConfigurationBuilder()
                .AddJsonFile(serviceConfigFile, true, true)
                .AddJsonFile(ocelotConsulConfigFile, false, true)
                .AddCommandLine(args)
                .AddEnvironmentVariables();

            builder = Configure(builder);

            var config = builder.Build();

            return new WebHostBuilder()
                .UseConfiguration(config)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<GatewayServiceStartup>()
                .UseKestrel()
                .Build();
        }
    }
}
