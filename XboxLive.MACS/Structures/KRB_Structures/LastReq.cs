using System;
using System.Collections.Generic;
using System.Text;
using XboxLive.MACS.ASN;

namespace XboxLive.MACS.Structures.KRB_Structures
{
    public class LastReq
    {
        public int lr_type { get; set; }
        public DateTime lr_value { get; set; }

        public LastReq()
        {

        }

        public AsnElt Encode()
        {
            AsnElt lr_typeElt = AsnElt.MakeInteger(lr_type);
            AsnElt lr_typeSeq = AsnElt.Make(AsnElt.SEQUENCE, lr_typeElt);
            lr_typeSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 0, lr_typeSeq);

            AsnElt lr_valueElt = AsnElt.MakeTimeAuto(lr_value);
            AsnElt lr_valueSeq = AsnElt.Make(AsnElt.SEQUENCE, lr_valueElt);
            lr_valueSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, lr_valueSeq);

            AsnElt seq = AsnElt.Make(AsnElt.SEQUENCE, lr_typeSeq, lr_valueSeq);
            AsnElt seq2 = AsnElt.Make(AsnElt.SEQUENCE, seq);

            return seq2;
        }
    }
}
