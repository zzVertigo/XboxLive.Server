using System;
using System.Linq;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Structures.PA_Structures;

namespace XboxLive.MACS.Structures
{
    public class PA_DATA
    {
        public PA_ENC_TIMESTAMP Decode2(AsnElt body)
        {
            // Had to cheat a bit on this - will fix later lol.
            var EncTimestamp_Bytes = body.Sub[0].Sub[2].Sub[1].Sub[0].GetOctetString().Skip(11).ToArray();

            dynamic result = new PA_ENC_TIMESTAMP
            {
                USec = null, // Not used lol
                Timestamp = EncTimestamp_Bytes
            };

            return result;
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
            byte[] Version_Buffer = new byte[65];

            byte[] Buffer206 = body.Sub[0].Sub[1].Sub[1].GetOctetString();

            Array.Copy(Buffer206, 0, Signature_Buffer, 0, 20);
            Array.Copy(Buffer206, 20, Version_Buffer, 0, 65);

            dynamic result = new PA_XBOX_CLIENT_VERSION
            {
                Version = Version_Buffer,
                Signature = Signature_Buffer
            };

            return result;
        }
    }
}
