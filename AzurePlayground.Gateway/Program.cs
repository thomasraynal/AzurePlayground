using Microsoft.AspNetCore.Hosting;

namespace AzurePlayground.Gateway
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = new GatewayHost();

            var app = host.Build(args);

            app.Start();
            app.WaitForShutdown();
        }
    }
}
