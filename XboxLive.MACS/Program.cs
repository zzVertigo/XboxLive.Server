using System;
using System.Diagnostics;
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

        private static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();

            var staticData = new StaticData();

            staticData.LoadConfig();

            Console.Title = "Xbox Live MACS Server";

            Resources.Start();

            XServer server = new XServer(staticData.ServerOptions);
            server.Start();

            watch.Stop();

            Logger.Info($"Server started in {watch.Elapsed.TotalMilliseconds} ms");

            Console.ReadKey();
        }
    }
}