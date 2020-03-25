using XboxLive.MACS.ASN;

namespace XboxLive.MACS.Structures.KRB_Structures
{
    public class EncryptedData
    {
        public EncryptedData(int encType, int Kvno, byte[] data)
        {
            etype = encType;
            cipher = data;
            kvno = Kvno;
        }

        public int etype { get; set; }
        public int kvno { get; set; }
        public byte[] cipher { get; set; }

        public AsnElt Encode()
        {
            var etypeAsn = AsnElt.MakeInteger(etype);
            var etypeSeq = AsnElt.Make(AsnElt.SEQUENCE, etypeAsn);
            etypeSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 0, etypeSeq);


            var cipherAsn = AsnElt.MakeBlob(cipher);
            var cipherSeq = AsnElt.Make(AsnElt.SEQUENCE, cipherAsn);
            cipherSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, cipherSeq);


            if (kvno != 0)
            {
                var kvnoAsn = AsnElt.MakeInteger(kvno);
                var kvnoSeq = AsnElt.Make(AsnElt.SEQUENCE, kvnoAsn);
                kvnoSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 2, kvnoSeq);

                var totalSeq = AsnElt.Make(AsnElt.SEQUENCE, etypeSeq, kvnoSeq, cipherSeq);
                return totalSeq;
            }
            else
            {
                var totalSeq = AsnElt.Make(AsnElt.SEQUENCE, etypeSeq, cipherSeq);
                return totalSeq;
            }
        }
    }
}