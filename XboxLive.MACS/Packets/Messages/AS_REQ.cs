using System;
using System.Text;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Core;
using XboxLive.MACS.Structures;
using XboxLive.MACS.Structures.PA_Structures;

namespace XboxLive.MACS.Packets.Messages
{
    public class AS_REQ : Message
    {
        public PA_XBOX_CLIENT_VERSION PA_XBOX_CLIENT_VERSION { get; set; }

        public Interop.KdcOptions KDC_OPTIONS { get; set; }
        
        public long CNAME_TYPE { get; set; }
        public string CNAME { get; set; }

        public string REALM { get; set; }

        public long SNAME_TYPE { get; set; }
        public string SNAME1 { get; set; }
        public string SNAME2 { get; set; }

        public DateTime TILL { get; set; }

        public long NONCE { get; set; }

        public Interop.KERB_ETYPE ENC_TYPE { get; set; }

        public AS_REQ(XClient client) : base(client)
        {
        }

        public override void Decode()
        {
            // PA_DATA -> SEQUENCE -> SEQUENCE -> Index of Sequence -> Index of Value
            // REQ_BODY -> SEQUENCE -> Index of Sequence -> Index of Value

            // TODO: Decode PA_DATA

            //var PA_ENC_TIMESTAMP = new PA_DATA().Decode2(PA_DATA);
            //var PA_PAC_REQUEST_EX = new PA_DATA().Decode131(PA_DATA);
            //var PA_XBOX_PRE_PRE_AUTH = new PA_DATA().Decode204(PA_DATA);
            PA_XBOX_CLIENT_VERSION = new PA_DATA().Decode206(PA_DATA);

            KDC_OPTIONS = (Interop.KdcOptions) REQ_BODY.Sub[0].Sub[0].Sub[0].GetInteger();

            CNAME_TYPE = REQ_BODY.Sub[0].Sub[1].Sub[0].Sub[0].Sub[0].GetInteger();
            CNAME = REQ_BODY.Sub[0].Sub[1].Sub[0].Sub[1].Sub[0].Sub[0].GetString();

            REALM = REQ_BODY.Sub[0].Sub[2].Sub[0].GetString();

            SNAME_TYPE = REQ_BODY.Sub[0].Sub[3].Sub[0].Sub[0].Sub[0].GetInteger();
            SNAME1 = REQ_BODY.Sub[0].Sub[3].Sub[0].Sub[1].Sub[0].Sub[0].GetString();
            SNAME2 = REQ_BODY.Sub[0].Sub[3].Sub[0].Sub[1].Sub[0].Sub[1].GetString();

            TILL = REQ_BODY.Sub[0].Sub[4].Sub[0].GetTime();

            NONCE = REQ_BODY.Sub[0].Sub[5].Sub[0].GetInteger();

            ENC_TYPE = (Interop.KERB_ETYPE) REQ_BODY.Sub[0].Sub[6].Sub[0].Sub[0].GetInteger();
        }

        public override void Process()
        {
            //AsnElt pvnoASN = AsnElt.MakeInteger(5);
            //AsnElt pvnoSEQ = AsnElt.Make(AsnElt.SEQUENCE, pvnoASN);

            //pvnoSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, pvnoSEQ);

            //AsnElt msg_typeASN = AsnElt.MakeInteger(11);
            //AsnElt msg_typeSEQ = AsnElt.Make(AsnElt.SEQUENCE, msg_typeASN);

            //msg_typeSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 2, msg_typeSEQ);

            //AsnElt crealmASN = AsnElt.MakeString(12, "MACS.XBOX.COM");
            //AsnElt crealmSEQ = AsnElt.Make(AsnElt.SEQUENCE, crealmASN);

            //crealmSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 3, crealmSEQ);

            //AsnElt cnameASN = AsnElt.MakeString(27, "105581114003");
            //AsnElt cnameSEQ = AsnElt.Make(AsnElt.SEQUENCE, cnameASN);

            //cnameSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 4, cnameSEQ);

            //AsnElt[] total = new[] { pvnoSEQ, msg_typeSEQ, crealmSEQ, cnameSEQ };
            //AsnElt seq = AsnElt.Make(AsnElt.SEQUENCE, total);

            //AsnElt totalSeq = AsnElt.Make(AsnElt.SEQUENCE, seq);
            //totalSeq = AsnElt.MakeImplicit(AsnElt.APPLICATION, 11, totalSeq);

            //this.Client.Send(totalSeq.Encode());
        }

        public override string ToString()
        {
            return "AS_REQ";
        }
    }
}
