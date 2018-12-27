using AzurePlayground.Service;
using Dasein.Core.Lite;
using Microsoft.AspNetCore.Hosting;
using System;

namespace AzurePlayground.Authentication
{
    class Program
    {
        static void Main(string[] args)
        {
            args = new[] { "urls=http://*:5001" };

            var host = new Host<AuthenticationServiceStartup>();

            var app = host.Build(args);

            app.Start();
            app.WaitForShutdown();
        }
    }
}
