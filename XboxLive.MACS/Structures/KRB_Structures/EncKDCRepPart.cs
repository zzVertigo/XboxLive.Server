using System;
using System.Collections.Generic;
using XboxLive.MACS.ASN;

namespace XboxLive.MACS.Structures.KRB_Structures
{
    public class EncKDCRepPart
    {
        public EncryptionKey key { get; set; }
        public LastReq lastReq { get; set; }
        public uint nonce { get; set; }
        public Interop.TicketFlags flags { get; set; }
        public DateTime authtime { get; set; }
        public DateTime endtime { get; set; }
        public string realm { get; set; }
        public PrincipalName sname { get; set; }

        public AsnElt Encode()
        {
            var asnElements = new List<AsnElt>();

            var keyAsn = key.Encode();
            keyAsn = AsnElt.MakeImplicit(AsnElt.CONTEXT, 0, keyAsn);
            asnElements.Add(keyAsn);

            var lastReqAsn = lastReq.Encode();
            lastReqAsn = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, lastReqAsn);
            asnElements.Add(lastReqAsn);

            var nonceElt = AsnElt.MakeInteger(Convert.ToInt64(nonce));
            var nonceSeq = AsnElt.Make(AsnElt.SEQUENCE, nonceElt);
            nonceSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 2, nonceSeq);
            asnElements.Add(nonceSeq);

            var flagsElt = AsnElt.MakeInteger(Convert.ToInt32(flags));
            var flagsSeq = AsnElt.Make(AsnElt.SEQUENCE, flagsElt);
            flagsSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 4, flagsSeq);
            asnElements.Add(flagsSeq);

            var authtimeElt = AsnElt.MakeTimeAuto(authtime);
            var authtimeSeq = AsnElt.Make(AsnElt.SEQUENCE, authtimeElt);
            authtimeSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 5, authtimeSeq);
            asnElements.Add(authtimeSeq);

            var endtimeElt = AsnElt.MakeTimeAuto(endtime);
            var endtimeSeq = AsnElt.Make(AsnElt.SEQUENCE, endtimeElt);
            endtimeSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 7, endtimeSeq);
            asnElements.Add(endtimeSeq);

            // TODO: Find proper string type
            var realmElt = AsnElt.MakeString(12, realm);
            var realmSeq = AsnElt.Make(AsnElt.SEQUENCE, realmElt);
            realmSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 9, realmSeq);
            asnElements.Add(realmSeq);

            sname = new PrincipalName(new List<string>
            {
                "krbgt",
                "MACS.XBOX.COM"
            }, 2);

            var snameElt = sname.Encode();
            snameElt = AsnElt.MakeImplicit(AsnElt.CONTEXT, 10, snameElt);
            asnElements.Add(snameElt);

            var seq = AsnElt.Make(AsnElt.SEQUENCE, asnElements.ToArray());

            return seq;
        }
    }
}