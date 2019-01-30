using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dasein.Core.Lite;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzurePlayground.Web.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new Host<WebAppStartup>();
            var app = host.Build(args);

            app.Start();
            app.WaitForShutdown();
        }
    }
}
