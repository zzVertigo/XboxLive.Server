using System.Collections.Generic;
using XboxLive.MACS.ASN;

namespace XboxLive.MACS.Structures.KRB_Structures
{
    public class PrincipalName
    {
        public PrincipalName()
        {
            name_type = 1;
            name_string = new List<string>();
        }

        public PrincipalName(string principal, int name_type)
        {
            this.name_type = name_type;

            name_string = new List<string>();
            name_string.Add(principal);
        }

        public PrincipalName(List<string> names, int name_type)
        {
            this.name_type = name_type;

            name_string = names;
        }

        public long name_type { get; set; }
        public List<string> name_string { get; set; }

        public AsnElt Encode()
        {
            var nameTypeElt = AsnElt.MakeInteger(name_type);
            var nameTypeSeq = AsnElt.Make(AsnElt.SEQUENCE, nameTypeElt);
            nameTypeSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 0, nameTypeSeq);

            var strings = new AsnElt[name_string.Count];

            for (var i = 0; i < name_string.Count; ++i)
            {
                var name = name_string[i];
                var nameStringElt = AsnElt.MakeString(AsnElt.IA5String, name);
                nameStringElt = AsnElt.MakeImplicit(AsnElt.UNIVERSAL, AsnElt.GeneralString, nameStringElt);
                strings[i] = nameStringElt;
            }

            var stringSeq = AsnElt.Make(AsnElt.SEQUENCE, strings);
            var stringSeq2 = AsnElt.Make(AsnElt.SEQUENCE, stringSeq);
            stringSeq2 = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, stringSeq2);

            var seq = AsnElt.Make(AsnElt.SEQUENCE, nameTypeSeq, stringSeq2);

            var seq2 = AsnElt.Make(AsnElt.SEQUENCE, seq);

            return seq2;
        }
    }
}