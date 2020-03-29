using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Core;
using XboxLive.MACS.Core.Configuration;

namespace XboxLive.MACS
{
    public class Program
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static IConfigurationRoot Configuration;

        private static void Main(string[] args)
        {
            // Init service collection
            var services = new ServiceCollection();

            // Configure services (setup DI)
            ConfigureServices(services);

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            Stopwatch watch = new Stopwatch();

            watch.Start();

            Console.Title = "Xbox Live MACS Server";

            Resources.Start();

            var server = serviceProvider.GetService<XServer>();

            server.Start();

            watch.Stop();

            Logger.Info($"Server started in {watch.Elapsed.TotalMilliseconds} ms");

            Console.ReadKey();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            services.Configure<ServerOptions>(Configuration.GetSection("ServerOptions"));
            services.AddSingleton<IConfigurationRoot>(Configuration);
            services.AddSingleton<XServer>();
        }
    }
}