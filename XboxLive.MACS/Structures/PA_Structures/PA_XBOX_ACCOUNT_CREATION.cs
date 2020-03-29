using System;
using System.IO;
using NLog;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Crypto;
using XboxLive.MACS.Structures.KRB_Structures;

namespace XboxLive.MACS.Structures.PA_Structures
{
    public class PA_XBOX_ACCOUNT_CREATION
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public long PUID { get; set; } // 1 (ex)
        public string GamerTag { get; set; } // SN.XXXXXXXXXXXX (Console Serial Number)
        public string Domain { get; set; } // xbox.com
        public string Realm { get; set; } // passport.net
        public byte[] Key { get; set; } // MD4 hashed key, not the password

        public AsnElt Encode(long puid, string gamertag, string domain, string realm, byte[] key)
        {
            byte[] machine_account_buffer = new byte[84]; // i think?

            byte[] bytes = BitConverter.GetBytes(puid);

            Array.Reverse(bytes);

            long result = BitConverter.ToInt64(bytes); // honestly might not matter THAT much but doing it anyways

            var machineaccount = new BinaryWriter(new MemoryStream(machine_account_buffer));
            {
                machineaccount.Write((long)result);
                machineaccount.Write((string)gamertag);
                machineaccount.Write((string)domain);
                machineaccount.Write((string)realm);
                machineaccount.Write((byte[])key);
            }

            machineaccount.Close();

            Logger.Info("Machine Account Buffer: " + BitConverter.ToString(machine_account_buffer).Replace("-", ""));

            byte[] encBytes = KerberosCrypto.KerberosEncrypt(Interop.KERB_ETYPE.rc4_hmac, 1203, key, machine_account_buffer);

            Logger.Warn("Length of ciphertext: " + encBytes.Length);


            AsnElt typeELT = AsnElt.MakeInteger((long) 203);
            AsnElt nameTypeSEQ = AsnElt.Make(AsnElt.SEQUENCE, new AsnElt[] {typeELT});
            nameTypeSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, nameTypeSEQ);

            EncryptedData encdata = new EncryptedData((int)Interop.KERB_ETYPE.rc4_hmac, 1, encBytes);

            AsnElt blob = AsnElt.MakeBlob(((EncryptedData)encdata).Encode().Encode());
            AsnElt blobSEQ = AsnElt.Make(AsnElt.SEQUENCE, new AsnElt[] {blob});
            blobSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 2, blobSEQ);

            AsnElt seq = AsnElt.Make(AsnElt.SEQUENCE, new AsnElt[] {nameTypeSEQ, blobSEQ});

            return seq;
        }
    }
}