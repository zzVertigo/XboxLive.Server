using System;
using System.Diagnostics;
using NLog;
using XboxLive.MACS.Core;
using XboxLive.MACS.Core.Configuration;

namespace XboxLive.MACS
{
    public class Program
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {
            var watch = new Stopwatch();

            watch.Start();

            var staticData = new StaticData();

            staticData.LoadConfig();

            Console.Title = "Xbox Live MACS Server";

            Resources.Start();

            var server = new XServer(staticData.ServerOptions);
            server.Start();

            watch.Stop();

            Logger.Info($"Server started in {watch.Elapsed.TotalMilliseconds} ms");

            Console.ReadKey();
        }
    }
}