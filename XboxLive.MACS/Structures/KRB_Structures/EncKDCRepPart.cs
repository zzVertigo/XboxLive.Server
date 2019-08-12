using System;
using System.Collections.Generic;
using System.Text;
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

        public EncKDCRepPart()
        {

        }

        public AsnElt Encode()
        {
            List<AsnElt> asnElements = new List<AsnElt>();

            AsnElt keyAsn = key.Encode();
            keyAsn = AsnElt.MakeImplicit(AsnElt.CONTEXT, 0, keyAsn);
            asnElements.Add(keyAsn);

            AsnElt lastReqAsn = lastReq.Encode();
            lastReqAsn = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, lastReqAsn);
            asnElements.Add(lastReqAsn);

            AsnElt nonceElt = AsnElt.MakeInteger(Convert.ToInt64(nonce));
            AsnElt nonceSeq = AsnElt.Make(AsnElt.SEQUENCE, nonceElt);
            nonceSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 2, nonceSeq);
            asnElements.Add(nonceSeq);

            AsnElt flagsElt = AsnElt.MakeInteger(Convert.ToInt32(flags));
            AsnElt flagsSeq = AsnElt.Make(AsnElt.SEQUENCE, flagsElt);
            flagsSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 4, flagsSeq);
            asnElements.Add(flagsSeq);

            AsnElt authtimeElt = AsnElt.MakeTimeAuto(authtime);
            AsnElt authtimeSeq = AsnElt.Make(AsnElt.SEQUENCE, authtimeElt);
            authtimeSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 5, authtimeSeq);
            asnElements.Add(authtimeSeq);

            AsnElt endtimeElt = AsnElt.MakeTimeAuto(endtime);
            AsnElt endtimeSeq = AsnElt.Make(AsnElt.SEQUENCE, endtimeElt);
            endtimeSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 7, endtimeSeq);
            asnElements.Add(endtimeSeq);

            // TODO: Find proper string type
            AsnElt realmElt = AsnElt.MakeString(12, realm);
            AsnElt realmSeq = AsnElt.Make(AsnElt.SEQUENCE, realmElt);
            realmSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 9, realmSeq);
            asnElements.Add(realmSeq);

            sname = new PrincipalName(new List<string>
            {
                "krbgt",
                "MACS.XBOX.COM"
            }, 2);

            AsnElt snameElt = sname.Encode();
            snameElt = AsnElt.MakeImplicit(AsnElt.CONTEXT, 10, snameElt);
            asnElements.Add(snameElt);

            AsnElt seq = AsnElt.Make(AsnElt.SEQUENCE, asnElements.ToArray());

            return seq;
        }
    }
}
