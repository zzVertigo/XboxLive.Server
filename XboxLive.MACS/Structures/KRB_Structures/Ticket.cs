using System.Collections.Generic;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Crypto;

namespace XboxLive.MACS.Structures.KRB_Structures
{
    public class Ticket
    {
        public int tkt_vno { get; set; }

        public string realm { get; set; }

        public PrincipalName sname { get; set; }

        public EncryptedData encdata { get; set; }

        public AsnElt Encode(byte[] OnlineKey)
        {
            AsnElt tkt_vnoAsn = AsnElt.MakeInteger(tkt_vno);
            AsnElt tkt_vnoSeq = AsnElt.Make(AsnElt.SEQUENCE, new AsnElt[] { tkt_vnoAsn });
            tkt_vnoSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 0, tkt_vnoSeq);

            AsnElt realmAsn = AsnElt.MakeString(AsnElt.IA5String, realm);
            realmAsn = AsnElt.MakeImplicit(AsnElt.UNIVERSAL, AsnElt.GeneralString, realmAsn);
            AsnElt realmAsnSeq = AsnElt.Make(AsnElt.SEQUENCE, realmAsn);
            realmAsnSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, realmAsnSeq);

            AsnElt snameAsn = sname.Encode();
            snameAsn = AsnElt.MakeImplicit(AsnElt.CONTEXT, 2, snameAsn);

            AsnElt enc_partAsn = encdata.Encode();
            AsnElt enc_partSeq = AsnElt.Make(AsnElt.SEQUENCE, enc_partAsn);
            enc_partSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 3, enc_partSeq);


            AsnElt totalSeq = AsnElt.Make(AsnElt.SEQUENCE, new[] { tkt_vnoSeq, realmAsnSeq, snameAsn, enc_partSeq });
            AsnElt totalSeq2 = AsnElt.Make(AsnElt.SEQUENCE, new[] { totalSeq });
            totalSeq2 = AsnElt.MakeImplicit(AsnElt.APPLICATION, 1, totalSeq2);

            return totalSeq2;
        }
    }
}