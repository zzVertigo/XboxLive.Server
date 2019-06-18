using System;
using System.Net;
using System.Net.Sockets;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Packets;

namespace XboxLive.MACS.Core
{
    public class XClient
    {
        public string SerialNumber { get; set; }
        public string Realm { get; set; }

        public DateTime Till { get; set; }

        public string Nonce { get; set; }

        public UdpClient Client { get; set; }

        public IPEndPoint RemoteEP { get; set; }

        // Decrypted Online Key (16 bytes)
        public byte[] OnlineKey =
        {
            0x0A, 0x1E, 0x35, 0x33, 0x71, 0x85, 0x31, 0x4D, 0x59, 0x12, 0x38, 0x48, 0x1C, 0x91, 0x53, 0x60
        };

        public XClient(UdpClient client, IPEndPoint remoteep)
        {
            this.Client = client;
            this.RemoteEP = remoteep;
        }

        public void Decode(byte[] data)
        {
            Console.WriteLine("Attempting to decode received packet..");

            try
            {
                AsnElt KerberosPacket = AsnElt.Decode(data);
                AsnElt[] Sequences = KerberosPacket.Sub[0].Sub;

                int PVNO = (int) Sequences[0].Sub[0].GetInteger();

                int MSG_TYPE = (int) Sequences[1].Sub[0].GetInteger();

                if (MSG_TYPE > 0 && PVNO == 5)
                {
                    AsnElt PA_DATA = Sequences[2];

                    AsnElt REQ_BODY = Sequences[3];

                    if (MessageFactory.Messages.ContainsKey(MSG_TYPE))
                    {
                        if (PA_DATA != null && REQ_BODY != null)
                        {
                            Message ReceviedMessage = (Message)Activator.CreateInstance(MessageFactory.Messages[MSG_TYPE], this);

                            ReceviedMessage.MSG_TYPE = MSG_TYPE;
                            ReceviedMessage.PA_DATA = PA_DATA;
                            ReceviedMessage.REQ_BODY = REQ_BODY;

                            ReceviedMessage.Decode();
                            ReceviedMessage.Process();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Received unknown msg_type = " + MSG_TYPE + "!");
                    }
                }

                //Console.WriteLine(BitConverter.ToString(PA_DATA.Sub[0].Sub[0].Sub[1].Sub[0].GetOctetString()));
            }
            catch (Exception Ex)
            {
                Console.WriteLine("Error: " + Ex);

                return;
            }

            Console.WriteLine("Successfully decoded packet header!");

            //AsnElt[] KDC_REQ = KRB_REQ.Sub[0].Sub;

            //int PVNO = (int) KDC_REQ[0].Sub[0].GetInteger();

            //// Protocol Version Number 5!
            //if (PVNO == 5)
            //{
            //    int MSG_TYPE = (int)KDC_REQ[1].Sub[0].GetInteger();
            //    var PA_DATA = KDC_REQ[2];
            //    var REQ_BODY = KDC_REQ[3];

            //    if (MessageFactory.Messages.ContainsKey(MSG_TYPE))
            //    {
            //        var ReceviedMessage = (Message)Activator.CreateInstance(MessageFactory.Messages[MSG_TYPE], this);

            //        ReceviedMessage.MSG_TYPE = MSG_TYPE;
            //        ReceviedMessage.PA_DATA = PA_DATA;
            //        ReceviedMessage.REQ_BODY = REQ_BODY;

            //        ReceviedMessage.Decode();
            //        ReceviedMessage.Process();
            //    }
            //    else
            //    {
            //        Console.WriteLine("Unknown MSG_TYPE received: " + MSG_TYPE);
            //    }
            //}
        }

        public void Send(byte[] data)
        {
            this.Client.Send(data, data.Length, RemoteEP);
        }
    }
}
