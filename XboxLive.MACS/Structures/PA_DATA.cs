using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Structures.PA_Structures;

namespace XboxLive.MACS.Structures
{
    public class PA_DATA
    {
        public object Decode2(AsnElt body)
        {
            return null;
        }

        // Not needed
        public object Decode131(AsnElt body)
        {
            return null;
        }

        // Used for verification
        public object Decode204(AsnElt body)
        {
            byte[] stuff = body.Sub[0].Sub[0].Sub[1].GetOctetString();

            byte[] FILE_TIME = stuff.Take(8).ToArray();

            return null;
        }

        public PA_XBOX_CLIENT_VERSION Decode206(AsnElt body)
        {
            byte[] Signature_Buffer = new byte[20];
            byte[] Version_Buffer = new byte[64];

            byte[] Buffer206 = body.Sub[0].Sub[1].Sub[1].GetOctetString();

            Array.Copy(Buffer206, 0, Signature_Buffer, 0, 20);
            Array.Copy(Buffer206, 20, Version_Buffer, 0, 64);

            dynamic result = new PA_XBOX_CLIENT_VERSION
            {
                Version = Version_Buffer,
                Signature = Signature_Buffer
            };

            return result;
        }
    }
}
