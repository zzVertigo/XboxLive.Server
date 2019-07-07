using System;
using System.IO;
using System.Linq;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Structures.PA_Structures;

namespace XboxLive.MACS.Structures
{
    public class PA_DATA
    {
        public AsnElt Encode203(long uniqueid, string gamertag, string domain, string realm, byte[] key)
        {
            byte[] buffer = new byte[84];

            BinaryWriter machineaccount = new BinaryWriter(new MemoryStream(buffer));
            machineaccount.Write((long)uniqueid);
            machineaccount.Write((string)gamertag);
            machineaccount.Write((string)domain);
            machineaccount.Write((string)realm);
            machineaccount.Write((byte[])key);

            machineaccount.Close();

            AsnElt etypeAsn = AsnElt.MakeInteger((int)Interop.KERB_ETYPE.rc4_hmac);
            AsnElt etypeSeq = AsnElt.Make(AsnElt.SEQUENCE, etypeAsn);

            etypeSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 0, etypeSeq);

            AsnElt cipherAsn = AsnElt.MakeBlob(buffer);
            AsnElt cipherSeq = AsnElt.Make(AsnElt.SEQUENCE, cipherAsn);

            cipherSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 2, cipherSeq);

            AsnElt totalSeq = AsnElt.Make(AsnElt.SEQUENCE, etypeSeq, cipherSeq);

            return totalSeq;
        }

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
