using XboxLive.MACS.ASN;
using XboxLive.MACS.Core;

namespace XboxLive.MACS.Packets
{
    public class ClientMessage
    {
        public XClient Client;

        public int MSG_TYPE;

        public AsnElt PA_DATA;
        public AsnElt REQ_BODY;

        public ClientMessage(XClient client)
        {
            this.Client = client;
        }

        public virtual void Decode()
        {
        }

        public virtual void Process()
        {
        }
    }

    public class ServerMessage
    {
        public XClient Client;

        public byte[] ResponseData;

        public ServerMessage(XClient client)
        {
            this.Client = client;
        }

        public virtual byte[] Encode()
        {
            return null;
        }
        public virtual void Process()
        {
        }
    }
}