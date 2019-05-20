using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using XboxLive.MACS.ASN;

namespace XboxLive.MACS
{
    public class Program
    {
        private static void Main(string[] args)
        {
            // TODO: Arguments!

            Console.Title = "Xbox Live Machine Account Creation Server";

            var udpServer = new UdpClient(88);

            while (true)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 88);
                byte[] data = udpServer.Receive(ref remoteEP);

                Console.WriteLine("Received " + data.Length + " bytes from " + remoteEP);

                Console.WriteLine("Data: " + BitConverter.ToString(data).Replace("-", ""));

                // First Request is AS_REQ

                data = AsnIO.FindBER(data);
                AsnElt AS_REQ = AsnElt.Decode(data);

                AsnElt[] KDC_REQ = AS_REQ.Sub[0].Sub;

                foreach (AsnElt s in KDC_REQ)
                {
                    switch (s.TagValue)
                    {
                        case 1:
                            Console.WriteLine("PVNO: " + s.Sub[0].GetInteger());
                            break;
                        case 2:
                            Console.WriteLine("MSG TYPE: " + s.Sub[0].GetInteger());
                            break;
                        case 3:
                            Console.WriteLine("PA DATA");
                            foreach (AsnElt pa in s.Sub[0].Sub)
                            {
                                Console.WriteLine(pa);
                            }
                            break;
                        case 4:
                            Console.WriteLine("KDC REQ BODY");
                            foreach (AsnElt b in s.Sub[0].Sub)
                            {
                                switch (b.TagValue)
                                {
                                    case 0:
                                        UInt32 temp = Convert.ToUInt32(b.Sub[0].GetInteger());
                                        byte[] tempBytes = BitConverter.GetBytes(temp);
                                        Console.WriteLine("KDC Options: " + (Interop.KdcOptions)BitConverter.ToInt32(tempBytes, 0));
                                        break;
                                    case 1: // Optional
                                        long CNameType = b.Sub[0].Sub[0].Sub[0].GetInteger();
                                        int CNumberOfNames = b.Sub[0].Sub[1].Sub[0].Sub.Length;

                                        Console.WriteLine("CName: ");
                                        Console.WriteLine("- Name Type " + CNameType);
                                        Console.WriteLine("- Number of Names " + CNumberOfNames);
                                        Console.Write("- Names: ");

                                        for (int i = 0; i < CNumberOfNames; i++)
                                        {
                                            Console.WriteLine( Encoding.ASCII.GetString(b.Sub[0].Sub[1].Sub[0].Sub[i].GetOctetString()) + " ");
                                        }
                                        break;
                                    case 2:
                                        Console.WriteLine("Realm: " + Encoding.ASCII.GetString(b.Sub[0].GetOctetString()));
                                        break;
                                    case 3:
                                        long SNameType = b.Sub[0].Sub[0].Sub[0].GetInteger();
                                        int SNumberOfNames = b.Sub[0].Sub[1].Sub[0].Sub.Length;

                                        Console.WriteLine("SName: ");
                                        Console.WriteLine("- Name Type " + SNameType);
                                        Console.WriteLine("- Number of Names " + SNumberOfNames);
                                        Console.Write("- Names: ");

                                        for (int i = 0; i < SNumberOfNames; i++)
                                        {
                                            Console.Write(Encoding.ASCII.GetString(b.Sub[0].Sub[1].Sub[0].Sub[i].GetOctetString()) + " ");
                                        }
                                        break;
                                    case 5:
                                        Console.WriteLine("\nTill: " + b.Sub[0].GetTime());
                                        break;
                                    case 7:
                                        Console.WriteLine("Nonce: " + Convert.ToUInt32(b.Sub[0].GetInteger()));
                                        break;
                                    case 8:
                                        Console.WriteLine("ETypes: ");

                                        for (int i = 0; i < b.Sub[0].Sub.Length; i++)
                                        {
                                            Console.WriteLine(" - " + (Interop.KERB_ETYPE)b.Sub[0].Sub[i].GetInteger());
                                        }
                                        break;
                                    default:
                                        throw new Exception("Unknown Tag Value!");
                                        break;
                                }
                            }
                            break;
                        default:
                            throw new Exception("Invalid tag AS_REQ value: " + s.TagValue);
                            break;
                    }
                }
            }
        }
    }
}