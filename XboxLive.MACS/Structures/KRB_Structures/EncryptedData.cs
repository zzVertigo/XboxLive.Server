using System;
using System.Collections.Generic;
using System.Text;
using XboxLive.MACS.ASN;

namespace XboxLive.MACS.Structures.KRB_Structures
{
    public class EncryptedData
    {
        public int etype { get; set; }
        public int kvno { get; set; }
        public byte[] cipher { get; set; }

        public EncryptedData(int encType, int Kvno, byte[] data)
        {
            etype = encType;
            cipher = data;
            kvno = Kvno;
        }

        public AsnElt Encode()
        {
            AsnElt etypeAsn = AsnElt.MakeInteger(etype);
            AsnElt etypeSeq = AsnElt.Make(AsnElt.SEQUENCE, etypeAsn);
            etypeSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 0, etypeSeq);

            
            AsnElt cipherAsn = AsnElt.MakeBlob(cipher);
            AsnElt cipherSeq = AsnElt.Make(AsnElt.SEQUENCE, cipherAsn);
            cipherSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, cipherSeq);


            if (kvno != 0)
            {
                AsnElt kvnoAsn = AsnElt.MakeInteger(kvno);
                AsnElt kvnoSeq = AsnElt.Make(AsnElt.SEQUENCE, kvnoAsn);
                kvnoSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 2, kvnoSeq);

                AsnElt totalSeq = AsnElt.Make(AsnElt.SEQUENCE, etypeSeq, kvnoSeq, cipherSeq);
                return totalSeq;
            }
            else
            {
                AsnElt totalSeq = AsnElt.Make(AsnElt.SEQUENCE, etypeSeq, cipherSeq);
                return totalSeq;
            }
        }
    }
}
