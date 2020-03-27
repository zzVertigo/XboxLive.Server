using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Core;
using XboxLive.MACS.Structures;
using XboxLive.MACS.Structures.KRB_Structures;

namespace XboxLive.MACS.Packets.Messages.Server
{
    public class AS_REP : ServerMessage
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public long pvno { get; set; }
        public long msg_type { get; set; }
        public string crealm { get; set; }
        public PrincipalName cname { get; set; }
        public Ticket ticket { get; set; }
        public EncryptedData enc_part { get; set; }

        public AsnElt accountInfo { get; set; }

        public AS_REP(XClient client) : base(client)
        {
        }

        public override byte[] Encode()
        {
            // null should be hmac??
            // accountInfo = new PA_DATA().Encode203(1, Client.GamerTag, Client.Domain, Client.Realm, null);

            AsnElt pvnoASN = AsnElt.MakeInteger(5);
            AsnElt pvnoSEQ = AsnElt.Make(AsnElt.SEQUENCE, new[] {pvnoASN});
            pvnoSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, pvnoSEQ);

            AsnElt[] total = new[] {pvnoSEQ};
            var seq = AsnElt.Make(AsnElt.SEQUENCE, total);

            AsnElt totalSeq = AsnElt.Make(AsnElt.SEQUENCE, new [] {seq});
            totalSeq = AsnElt.MakeImplicit(AsnElt.APPLICATION, 11, totalSeq);

            return totalSeq.Encode();
        }

        public override string ToString()
        {
            return "AS_REP";
        }
    }
}
