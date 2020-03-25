using XboxLive.MACS.ASN;

namespace XboxLive.MACS.Structures.KRB_Structures
{
    public class EncryptionKey
    {
        public int keytype { get; set; }
        public byte[] keyvalue { get; set; }

        public AsnElt Encode()
        {
            var keyTypeElt = AsnElt.MakeInteger(keytype);
            var keyTypeSeq = AsnElt.Make(AsnElt.SEQUENCE, keyTypeElt);
            keyTypeSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 0, keyTypeSeq);

            var blob = AsnElt.MakeBlob(keyvalue);
            var blobSeq = AsnElt.Make(AsnElt.SEQUENCE, blob);
            blobSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, blobSeq);

            var seq = AsnElt.Make(AsnElt.SEQUENCE, keyTypeSeq, blobSeq);
            var seq2 = AsnElt.Make(AsnElt.SEQUENCE, seq);

            return seq2;
        }
    }
}