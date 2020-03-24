using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NLog;
using XboxLive.MACS.Core.Configuration;

namespace XboxLive.MACS.Core
{
    public class XServer
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private static Socket udpServer = null;
        private Thread serverThread = null;

        private static EndPoint remoteEndpoint = null;

        private byte[] receiveBuffer = null;

        private ServerOptions options;

        public XServer(ServerOptions serverOptions)
        {
            options = serverOptions;
        }

        public void Start()
        {
            receiveBuffer = new byte[2048];
            remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);

            udpServer = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            udpServer.Bind(new IPEndPoint(IPAddress.Parse(options.Address), options.Port));

            serverThread = new Thread(ReadCallback);
            serverThread.Start();

            Logger.Info("Machine Account Creation server is now listening on " + options.Port + "!");
        }

        public static int SendToClient(byte[] data)
        {
            int length = udpServer.SendTo(data, remoteEndpoint);

            return length;
        }

        public void ReadCallback()
        {
            XClient xclient = new XClient();

            while (true)
            {
                int length = udpServer.ReceiveFrom(receiveBuffer, ref remoteEndpoint);

                byte[] tempBuffer = new byte[length];

                Array.Copy(receiveBuffer, 0, tempBuffer, 0, length); // create a properly sized buffer :)

                xclient.Decode(tempBuffer);
            }

            //UdpClient listener = (UdpClient) Ar.AsyncState;
            //byte[] receivedBytes = listener.EndReceive(Ar, ref remoteEP);
            
            //// The assumption is remoteEP updates to the sending ip (XBOX)

            //XClient xclient = new XClient(listener, remoteEP);

            //Logger.Info("Adding new client (" + xclient.Client.LocalEndPoint + ") to dictionary!");

            //bool result = xClientDictionary.TryAdd(xclient, remoteEP);

            //if (result)
            //{
            //    xclient.Decode(receivedBytes);
            //}
            //else
            //{
            //    Logger.Warn("Server attempted to add duplicate client to " + xClientDictionary);
            //}
        }
    }
}
