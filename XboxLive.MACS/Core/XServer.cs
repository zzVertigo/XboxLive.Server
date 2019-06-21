using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace XboxLive.MACS.Core
{
    public class ClientState
    {
        public Socket client = null;
        public const int bufferSize = 2048;
        public byte[] buffer = new byte[bufferSize];
    }

    public class XServer
    {
        public Dictionary<XClient, IPEndPoint> XClientDictionary = null;

        private ManualResetEvent allDone = null;
        private UdpClient udpServer = null;
        private IPEndPoint remoteEP = null;

        public void Start()
        {
            // Max Clients
            this.XClientDictionary = new Dictionary<XClient, IPEndPoint>(5);

            this.allDone = new ManualResetEvent(false);

            this.udpServer = new UdpClient(88);
            this.remoteEP = new IPEndPoint(IPAddress.Parse("10.0.0.143"), 88);

            Console.WriteLine("XboxLive.MACS is online on port 88!");

            try
            {
                while (true)
                {
                    this.udpServer.BeginReceive(ReadCallback, this.udpServer);
                }
            }
            catch (Exception Ex)
            {
                // TODO: Handle errors properly xD

                //Console.WriteLine("ERROR: " + Ex.Message);
            }
        }

        public void ReadCallback(IAsyncResult Ar)
        {
            UdpClient listener = (UdpClient) Ar.AsyncState;
            byte[] receivedBytes = listener.EndReceive(Ar, ref remoteEP);
            
            // The assumption is remoteEP updates to the sending ip (XBOX)

            XClient client = new XClient(listener, remoteEP);

            Console.WriteLine("XSERVER: Adding new client with an IPv4 Address " + client.Client.Client.LocalEndPoint + " to dictionary!");

            bool result = XClientDictionary.TryAdd(client, remoteEP);

            Console.WriteLine("XSERVER: Result was " + result + "!");

            if (result)
            {
                client.Decode(receivedBytes);
            }
            else
            {
                Console.WriteLine("Server attempted to add duplicate client to " + XClientDictionary);
            }
        }
    }
}
