using System;
using XboxLive.MACS.ASN;

namespace XboxLive.MACS.Structures.KRB_Structures
{
    public class LastReq
    {
        public int lr_type { get; set; }
        public DateTime lr_value { get; set; }

        public AsnElt Encode()
        {
            var lr_typeElt = AsnElt.MakeInteger(lr_type);
            var lr_typeSeq = AsnElt.Make(AsnElt.SEQUENCE, lr_typeElt);
            lr_typeSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 0, lr_typeSeq);

            var lr_valueElt = AsnElt.MakeTimeAuto(lr_value);
            var lr_valueSeq = AsnElt.Make(AsnElt.SEQUENCE, lr_valueElt);
            lr_valueSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, lr_valueSeq);

            var seq = AsnElt.Make(AsnElt.SEQUENCE, lr_typeSeq, lr_valueSeq);
            var seq2 = AsnElt.Make(AsnElt.SEQUENCE, seq);

            return seq2;
        }
    }
}