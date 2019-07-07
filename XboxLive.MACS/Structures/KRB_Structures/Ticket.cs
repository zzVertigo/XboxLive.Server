using System.Collections.Generic;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Crypto;

namespace XboxLive.MACS.Structures.KRB_Structures
{
    public class Ticket
    {
        public PrincipalName sname { get; set; }
        public EncryptedData encdata { get; set; }

        public byte[] SessionKey =
        {
            0xD, 0xE, 0xA, 0xD, 0xB, 0xE, 0xE, 0xF
        };

        public AsnElt Encode(byte[] OnlineKey)
        {
            List<string> snames = new List<string>()
            {
                "krbtgt",
                "MACS.XBOX.COM"
            };

            sname = new PrincipalName(snames);

            AsnElt tkt_vnoAsn = AsnElt.MakeInteger(5);
            AsnElt tkt_vnoSeq = AsnElt.Make(AsnElt.SEQUENCE, new AsnElt[] { tkt_vnoAsn });
            tkt_vnoSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 0, tkt_vnoSeq);


            // realm           [1] Realm
            AsnElt realmAsn = AsnElt.MakeString(AsnElt.IA5String, "PASSPORT.NET");
            realmAsn = AsnElt.MakeImplicit(AsnElt.UNIVERSAL, AsnElt.GeneralString, realmAsn);
            AsnElt realmAsnSeq = AsnElt.Make(AsnElt.SEQUENCE, realmAsn);
            realmAsnSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, realmAsnSeq);


            // sname           [2] PrincipalName
            AsnElt snameAsn = sname.Encode();
            snameAsn = AsnElt.MakeImplicit(AsnElt.CONTEXT, 2, snameAsn);

            encdata = new EncryptedData((int)Interop.KERB_ETYPE.rc4_hmac, KerberosCrypto.KerberosEncrypt(Interop.KERB_ETYPE.rc4_hmac, Interop.KRB_KEY_USAGE_AS_REP_EP_SESSION_KEY, OnlineKey, SessionKey));

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
