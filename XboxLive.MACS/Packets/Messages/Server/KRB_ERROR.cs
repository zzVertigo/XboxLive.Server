using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Core;
using XboxLive.MACS.Structures.KRB_Structures;

namespace XboxLive.MACS.Packets.Messages.Server
{
    public class KRB_ERROR : Message
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public PrincipalName sname;

        public KRB_ERROR(XClient client) : base(client)
        {
            this.MSG_TYPE = 30;
            this.Client = client;

            List<string> snames = new List<string>(2);
            {
                snames.Add("krbtgt");
                snames.Add("XBOX.COM");
            }

            this.sname = new PrincipalName(snames, 2);
        }

        public override byte[] Encode()
        {
            AsnElt pvnoASN = AsnElt.MakeInteger(this.PVNO);
            AsnElt pvnoSEQ = AsnElt.Make(AsnElt.SEQUENCE, pvnoASN);
            pvnoSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 0, pvnoSEQ);

            AsnElt msgtypeASN = AsnElt.MakeInteger(this.MSG_TYPE);
            AsnElt msgtypeSEQ = AsnElt.Make(AsnElt.SEQUENCE, msgtypeASN);
            msgtypeSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, msgtypeSEQ);

            AsnElt stimeASN = AsnElt.MakeTimeAuto(new DateTime());
            AsnElt stimeSEQ = AsnElt.Make(AsnElt.SEQUENCE, stimeASN);
            stimeSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 4, stimeSEQ);

            AsnElt susecASN = AsnElt.MakeInteger(0);
            AsnElt susecSEQ = AsnElt.Make(AsnElt.SEQUENCE, susecASN);
            susecSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 5, susecSEQ);

            AsnElt errorASN = AsnElt.MakeInteger(25);
            AsnElt errorSEQ = AsnElt.Make(AsnElt.SEQUENCE, errorASN);
            errorSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 6, errorSEQ);

            AsnElt realmASN = AsnElt.MakeString(AsnElt.IA5String, "PASSPORT.NET");
            realmASN = AsnElt.MakeImplicit(AsnElt.UNIVERSAL, AsnElt.GeneralString, realmASN);
            AsnElt realmSEQ = AsnElt.Make(AsnElt.SEQUENCE, realmASN);
            realmSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 9, realmSEQ);

            AsnElt snameELT = this.sname.Encode();
            snameELT = AsnElt.MakeImplicit(AsnElt.CONTEXT, 10, snameELT);

            AsnElt[] total = new[] { pvnoSEQ, msgtypeSEQ, stimeSEQ, susecSEQ, errorSEQ, realmSEQ, snameELT };
            var seq = AsnElt.Make(AsnElt.SEQUENCE, total);

            AsnElt totalSeq = AsnElt.Make(AsnElt.SEQUENCE, new[] { seq });
            totalSeq = AsnElt.MakeImplicit(AsnElt.APPLICATION, this.MSG_TYPE, totalSeq);

            byte[] toSend = totalSeq.Encode();

            return toSend;
        }

        public override void Process()
        {
        }
    }
}
