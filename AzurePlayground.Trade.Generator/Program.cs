using Dasein.Core.Lite;
using Microsoft.AspNetCore.Hosting;
using System;

namespace AzurePlayground.Generator
{
    public class Program
    {
        static void Main(string[] args)
        {
            var host = new Host<TradeGeneratorStartup>();

            var app = host.Build(args);

            app.Start();
            app.WaitForShutdown();
        }
    }
}
