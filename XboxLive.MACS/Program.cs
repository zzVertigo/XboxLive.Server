using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Core;

namespace XboxLive.MACS
{
    public class Program
    {
        private static void Main(string[] args)
        {
            // TODO: Arguments!

            Console.Title = "Xbox Live Machine Account Creation Server";

            Resources.Start();

            XServer server = new XServer();
            server.Start();

            Thread.Sleep(-1);

            //while (true)
            //{
            //    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 88);
            //    byte[] data = udpServer.Receive(ref remoteEP);

            //    Console.WriteLine("Received " + data.Length + " bytes from " + remoteEP);

            //    Console.WriteLine("Data: " + BitConverter.ToString(data).Replace("-", ""));

            //    // First Request is AS_REQ

            //    data = AsnIO.FindBER(data);
            //    AsnElt AS_REQ = AsnElt.Decode(data);

            //    AsnElt[] KDC_REQ = AS_REQ.Sub[0].Sub;

            //    foreach (AsnElt s in KDC_REQ)
            //    {
            //        switch (s.TagValue)
            //        {
            //            case 1:
            //                Console.WriteLine("PVNO: " + s.Sub[0].GetInteger());
            //                break;
            //            case 2:
            //                Console.WriteLine("MSG TYPE: " + s.Sub[0].GetInteger());
            //                break;
            //            case 3:
            //                Console.WriteLine("PA DATA");
            //                foreach (AsnElt pa in s.Sub[0].Sub)
            //                {
            //                    Console.WriteLine(pa);
            //                }
            //                break;
            //            case 4:
            //               
            //                break;
            //            default:
            //                throw new Exception("Invalid tag AS_REQ value: " + s.TagValue);
            //                break;
            //        }
            //    }
            //}
        }
    }
}