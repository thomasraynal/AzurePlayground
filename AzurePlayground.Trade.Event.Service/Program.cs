using AzurePlayground.Service.Infrastructure;
using Dasein.Core.Lite;
using Microsoft.AspNetCore.Hosting;
using System;

namespace AzurePlayground.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new Host<TradeEventServiceStartup>();

            var app = host.Build(args);

            app.Start();
            app.WaitForShutdown();
        }
    }
}
