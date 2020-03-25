using System.Collections.Generic;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Crypto;

namespace XboxLive.MACS.Structures.KRB_Structures
{
    public class Ticket
    {
        public byte[] SessionKey =
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF
        };

        public PrincipalName sname { get; set; }
        public EncryptedData encdata { get; set; }

        public AsnElt Encode(byte[] OnlineKey)
        {
            var snames = new List<string>
            {
                "krbtgt",
                "XBOX.COM"
            };

            var allNodes = new List<AsnElt>();

            sname = new PrincipalName(snames, 2);

            var tkt_vnoAsn = AsnElt.MakeInteger(5);
            var tkt_vnoSeq = AsnElt.Make(AsnElt.SEQUENCE, tkt_vnoAsn);
            tkt_vnoSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 0, tkt_vnoSeq);
            allNodes.Add(tkt_vnoSeq);

            // realm           [1] Realm
            var realmAsn = AsnElt.MakeString(AsnElt.IA5String, "xbox.com");
            realmAsn = AsnElt.MakeImplicit(AsnElt.UNIVERSAL, AsnElt.GeneralString, realmAsn);
            var realmAsnSeq = AsnElt.Make(AsnElt.SEQUENCE, realmAsn);
            realmAsnSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, realmAsnSeq);
            allNodes.Add(realmAsnSeq);

            // sname           [2] PrincipalName
            var snameAsn = sname.Encode();
            snameAsn = AsnElt.MakeImplicit(AsnElt.CONTEXT, 2, snameAsn);
            allNodes.Add(snameAsn);

            // TODO: Find proper key type
            encdata = new EncryptedData((int) Interop.KERB_ETYPE.rc4_hmac, 1,
                KerberosCrypto.KerberosEncrypt(Interop.KERB_ETYPE.rc4_hmac, Interop.KRB_KEY_USAGE_AS_REP_EP_SESSION_KEY,
                    OnlineKey, SessionKey));

            var enc_partAsn = encdata.Encode();
            var enc_partSeq = AsnElt.Make(AsnElt.SEQUENCE, enc_partAsn);
            enc_partSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 3, enc_partSeq);
            allNodes.Add(enc_partSeq);

            var seq = AsnElt.Make(AsnElt.SEQUENCE, allNodes.ToArray());
            var seq2 = AsnElt.Make(AsnElt.SEQUENCE, seq);
            var seq3 = AsnElt.Make(AsnElt.APPLICATION, 1, seq2);
            seq3 = AsnElt.MakeImplicit(AsnElt.APPLICATION, 0, seq3);

            return seq3;
        }
    }
}