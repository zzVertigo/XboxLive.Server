using System;
using System.Collections.Generic;
using System.Text;
using XboxLive.MACS.ASN;

namespace XboxLive.MACS.Structures.KRB_Structures
{
    public class EncryptionKey
    {
        public int keytype { get; set; }
        public byte[] keyvalue { get; set; }

        public EncryptionKey()
        {

        }

        public AsnElt Encode()
        {
            AsnElt keyTypeElt = AsnElt.MakeInteger(keytype);
            AsnElt keyTypeSeq = AsnElt.Make(AsnElt.SEQUENCE, keyTypeElt);
            keyTypeSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 0, keyTypeSeq);

            AsnElt blob = AsnElt.MakeBlob(keyvalue);
            AsnElt blobSeq = AsnElt.Make(AsnElt.SEQUENCE, blob);
            blobSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, blobSeq);

            AsnElt seq = AsnElt.Make(AsnElt.SEQUENCE, keyTypeSeq, blobSeq);
            AsnElt seq2 = AsnElt.Make(AsnElt.SEQUENCE, seq);

            return seq2;
        }
    }
}
