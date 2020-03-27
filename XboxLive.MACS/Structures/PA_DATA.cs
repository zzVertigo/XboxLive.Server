using System;
using System.IO;
using System.Linq;
using NLog;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Crypto;
using XboxLive.MACS.Structures.KRB_Structures;
using XboxLive.MACS.Structures.PA_Structures;

namespace XboxLive.MACS.Structures
{
    public class PA_DATA
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

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

        public PA_XBOX_CLIENT_VERSION Decode206(AsnElt body)
        {
            var Signature_Buffer = new byte[20];
            var Version_Buffer = new byte[65];

            var Buffer206 = body.Sub[0].Sub[1].Sub[1].GetOctetString();

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