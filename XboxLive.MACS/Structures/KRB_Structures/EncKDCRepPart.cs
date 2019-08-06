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
        public DateTime key_expiration { get; set; }
        public Interop.TicketFlags flags { get; set; }
        public DateTime authtime { get; set; }       
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public DateTime renew_till { get; set; }
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

            //AsnElt key_expirationElt = AsnElt.MakeTimeAuto(key_expiration);
            //AsnElt key_expirationSeq = AsnElt.Make(AsnElt.SEQUENCE, key_expirationElt);
            //key_expirationSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 3, key_expirationSeq);
            //asnElements.Add(key_expirationSeq);

            AsnElt flagsElt = AsnElt.MakeInteger(Convert.ToInt32(flags));
            AsnElt flagsSeq = AsnElt.Make(AsnElt.SEQUENCE, flagsElt);
            flagsSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 4, flagsSeq);
            asnElements.Add(flagsSeq);

            AsnElt authtimeElt = AsnElt.MakeTimeAuto(authtime);
            AsnElt authtimeSeq = AsnElt.Make(AsnElt.SEQUENCE, authtimeElt);
            authtimeSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 5, authtimeSeq);
            asnElements.Add(authtimeSeq);

            //AsnElt starttimeElt = AsnElt.MakeTimeAuto(starttime);
            //AsnElt starttimeSeq = AsnElt.Make(AsnElt.SEQUENCE, starttimeElt);
            //starttimeSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 6, starttimeSeq);
            //asnElements.Add(starttimeSeq);

            AsnElt endtimeElt = AsnElt.MakeTimeAuto(endtime);
            AsnElt endtimeSeq = AsnElt.Make(AsnElt.SEQUENCE, endtimeElt);
            endtimeSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 7, endtimeSeq);
            asnElements.Add(endtimeSeq);

            //AsnElt renew_tillElt = AsnElt.MakeTimeAuto(renew_till);
            //AsnElt renew_tillSeq = AsnElt.Make(AsnElt.SEQUENCE, renew_tillElt);
            //renew_tillSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 8, renew_tillSeq);
            //asnElements.Add(renew_tillSeq);

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
