using System.Collections.Generic;
using System.Text;
using XboxLive.MACS.ASN;

namespace XboxLive.MACS.Structures.KRB_Structures
{
    public class PrincipalName
    {
        public PrincipalName(string principal)
        {
            name_type = 1;
            name_string = new List<string>();

            name_string.Add(principal);
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

        public PrincipalName(AsnElt body)
        {
            // KRB_NT_PRINCIPAL = 1
            //      means just the name of the principal
            // KRB_NT_SRV_INST = 2
            //      service and other unique instance (krbtgt)

            name_type = body.Sub[0].Sub[0].GetInteger();

            int numberOfNames = body.Sub[1].Sub[0].Sub.Length;

            name_string = new List<string>();

            for (int i = 0; i < numberOfNames; i++)
            {
                name_string.Add(Encoding.ASCII.GetString(body.Sub[1].Sub[0].Sub[i].GetOctetString()));
            }
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