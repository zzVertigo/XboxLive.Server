using System;
using System.Net;
using System.Net.Sockets;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Packets;

namespace XboxLive.MACS.Core
{
    public class XClient
    {
        public long UniqueID { get; set; }

        public string SerialNumber { get; set; }

        public string GamerTag { get; set; }

        public string Realm { get; set; }

        public string Domain { get; set; }

        public byte[] Key { get; set; }

        public DateTime Till { get; set; }

        public long Nonce { get; set; }

        public UdpClient Client { get; set; }

        public IPEndPoint RemoteEP { get; set; }

        public XClient(UdpClient client, IPEndPoint remoteep)
        {
            this.Client = client;
            this.RemoteEP = remoteep;
        }

        public void Decode(byte[] data)
        {
            Console.WriteLine("XCLIENT: Attempting to decode received packet..");

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
                        Console.WriteLine("XCLIENT: Error -> Received unknown msg_type = " + MSG_TYPE + "!");
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine("XCLIENT: Error -> " + Ex);

                return;
            }

            Console.WriteLine("XCLIENT: Successfully decoded packet !");
        }

        public void Send(byte[] data)
        {
            this.Client.Send(data, data.Length, RemoteEP);
        }
    }
}
