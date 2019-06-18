using System;
using System.Collections.Generic;
using System.Text;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Core;

namespace XboxLive.MACS.Packets
{
    public class Message
    {
        public int MSG_TYPE;

        public AsnElt PA_DATA;
        public AsnElt REQ_BODY;

        public XClient Client;

        public Message(XClient client)
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
}
