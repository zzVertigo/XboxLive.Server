using System;
using System.Net.Sockets;
using NLog;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Packets;

namespace XboxLive.MACS.Core
{
    public class XClient : UdpClient
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public long UniqueID { get; set; }

        public string SerialNumber { get; set; }

        public string GamerTag { get; set; }

        public string Realm { get; set; }

        public string Domain { get; set; }

        public byte[] OnlineKey = new byte[16];

        public byte[] NonceHmacKey = new byte[16];

        public DateTime Till { get; set; }

        public long Nonce { get; set; }

        public const ulong EncryptionOverheadLength = 24; // sizeof(RC4_MDx_HEADER)

        public void Decode(byte[] data)
        {
            Logger.Info("Attemping to decode received packet..");

            try
            {
                var KerberosPacket = AsnElt.Decode(data);
                var Sequences = KerberosPacket.Sub[0].Sub;

                var PVNO = (int) Sequences[0].Sub[0].GetInteger();

                var MSG_TYPE = (int) Sequences[1].Sub[0].GetInteger();

                if (MSG_TYPE > 0 && PVNO == 5)
                {
                    var PA_DATA = Sequences[2];

                    var REQ_BODY = Sequences[3];

                    if (MessageFactory.Messages.ContainsKey(MSG_TYPE))
                    {
                        if (PA_DATA != null && REQ_BODY != null)
                        {
                            var ReceviedMessage =
                                (Message) Activator.CreateInstance(MessageFactory.Messages[MSG_TYPE], this);

                            ReceviedMessage.MSG_TYPE = MSG_TYPE;
                            ReceviedMessage.PA_DATA = PA_DATA;
                            ReceviedMessage.REQ_BODY = REQ_BODY;

                            ReceviedMessage.Decode();
                            ReceviedMessage.Process();
                        }
                    }
                    else
                    {
                        // If this happens I wanna KNOW its happening lol
                        Logger.Warn("Received unknown msg_type " + MSG_TYPE + "!");
                        Logger.Warn("Received unknown msg_type " + MSG_TYPE + "!");
                        Logger.Warn("Received unknown msg_type " + MSG_TYPE + "!");
                        Logger.Warn("Received unknown msg_type " + MSG_TYPE + "!");
                        Logger.Warn("Received unknown msg_type " + MSG_TYPE + "!");
                        Logger.Warn("Received unknown msg_type " + MSG_TYPE + "!");
                        Logger.Warn("Received unknown msg_type " + MSG_TYPE + "!");
                        Logger.Warn("Received unknown msg_type " + MSG_TYPE + "!");
                        Logger.Warn("Received unknown msg_type " + MSG_TYPE + "!");
                        Logger.Warn("Received unknown msg_type " + MSG_TYPE + "!");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to decode received packet : " + ex);

                return;
            }

            Logger.Info("Successfully decoded received packet!");
        }

        public void Send(Message Message)
        {
            byte[] data = Message.Encode();
            Message.Process();

            var length = XServer.SendToClient(data);

            if (length > 0)
                Logger.Info("Sent " + length + " bytes back to client!");
            else
                Logger.Warn("why are we sending no data??");
        }
    }
}