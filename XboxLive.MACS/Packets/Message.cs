using XboxLive.MACS.ASN;
using XboxLive.MACS.Core;

namespace XboxLive.MACS.Packets
{
    public class Message
    {
        public XClient Client;
        public int MSG_TYPE;

        public AsnElt PA_DATA;
        public AsnElt REQ_BODY;

        public Message(XClient client)
        {
            Client = client;
        }

        public virtual void Decode()
        {
        }

        public virtual void Process()
        {
        }
    }
}