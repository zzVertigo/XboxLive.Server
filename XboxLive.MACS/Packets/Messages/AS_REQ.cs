using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Core;
using XboxLive.MACS.Crypto;
using XboxLive.MACS.Structures;
using XboxLive.MACS.Structures.KRB_Structures;
using XboxLive.MACS.Structures.PA_Structures;

namespace XboxLive.MACS.Packets.Messages
{
    public class AS_REQ : Message
    {
        public PrincipalName cname { get; set; }
        public PrincipalName crealm { get; set; }
        public Ticket reqTicket { get; set; }

        // Decrypted Online Key (16 bytes) - Temp
        public byte[] OnlineKey =
        {
            0x4f, 0xf6, 0xea, 0xa3, 0x86, 0x08, 0xdd, 0xc5, 0x95, 0x08, 0x55, 0xbf, 0xee, 0xc7, 0xdd, 0x00
        };

        public byte[] SaltedNonce =
        {
            0x5F, 0xF3, 0x28, 0x92, 0x13, 0x8C, 0x9C, 0x4B, 0x05, 0x84, 0x9A, 0x3C, 0x10, 0x1A, 0xDB, 0x5D
        };

        public PA_XBOX_CLIENT_VERSION PA_XBOX_CLIENT_VERSION { get; set; }
        public PA_ENC_TIMESTAMP PA_ENC_TIMESTAMP { get; set; }

        public Interop.KdcOptions KDC_OPTIONS { get; set; }
        
        public long CNAME_TYPE { get; set; }
        public string CNAME { get; set; }

        public string REALM { get; set; }

        public long SNAME_TYPE { get; set; }
        public string SNAME1 { get; set; }
        public string SNAME2 { get; set; }

        public DateTime TILL { get; set; }

        public long NONCE { get; set; }

        public Interop.KERB_ETYPE ENC_TYPE { get; set; }

        public AS_REQ(XClient client) : base(client)
        {
        }

        public override void Decode()
        {
            PA_ENC_TIMESTAMP = new PA_DATA().Decode2(PA_DATA);
            PA_XBOX_CLIENT_VERSION = new PA_DATA().Decode206(PA_DATA);

            KDC_OPTIONS = (Interop.KdcOptions) REQ_BODY.Sub[0].Sub[0].Sub[0].GetInteger();

            CNAME_TYPE = REQ_BODY.Sub[0].Sub[1].Sub[0].Sub[0].Sub[0].GetInteger();
            CNAME = REQ_BODY.Sub[0].Sub[1].Sub[0].Sub[1].Sub[0].Sub[0].GetString();

            REALM = REQ_BODY.Sub[0].Sub[2].Sub[0].GetString();

            SNAME_TYPE = REQ_BODY.Sub[0].Sub[3].Sub[0].Sub[0].Sub[0].GetInteger();
            SNAME1 = REQ_BODY.Sub[0].Sub[3].Sub[0].Sub[1].Sub[0].Sub[0].GetString();
            SNAME2 = REQ_BODY.Sub[0].Sub[3].Sub[0].Sub[1].Sub[0].Sub[1].GetString();

            TILL = REQ_BODY.Sub[0].Sub[4].Sub[0].GetTime();

            NONCE = REQ_BODY.Sub[0].Sub[5].Sub[0].GetInteger();

            ENC_TYPE = (Interop.KERB_ETYPE) REQ_BODY.Sub[0].Sub[6].Sub[0].Sub[0].GetInteger();
        }

        public bool VerifyXClient(Interop.KdcOptions KDC_OPTIONS, string REALM, string SNAME1, string SNAME2,
            Interop.KERB_ETYPE ENC_TYPE)
        {
            return KDC_OPTIONS == Interop.KdcOptions.CANONICALIZE && REALM == "MACS.XBOX.COM" && SNAME1 == "krbtgt" &&
                   SNAME2 == "MACS.XBOX.COM" && ENC_TYPE == Interop.KERB_ETYPE.rc4_hmac;
        }

        public bool VerifyTSXClient(byte[] OnlineKey, byte[] EncryptedTS)
        {
            // Still figuring out how I am going to deal with this so I am going to cheat a bit :P

            byte[] dec_ts = KerberosCrypto.KerberosDecrypt(Interop.KERB_ETYPE.rc4_hmac,
                Interop.KRB_KEY_USAGE_AS_REQ_PA_ENC_TIMESTAMP, OnlineKey, EncryptedTS);

            string actualts = Encoding.UTF8.GetString(dec_ts.Skip(6).Take(15).ToArray());

            if (actualts != null)
                return true;

            return false;
        }

        public bool SignatureCheckXClient(byte[] OnlineKey, byte[] Signature)
        {
            HMACMD5 firsthash = new HMACMD5(OnlineKey);
            byte[] temp_key = firsthash.ComputeHash(Encoding.UTF8.GetBytes("signaturekey\0"));
            Console.WriteLine("SIGCHK: temp_key -> 0x" + BitConverter.ToString(temp_key).Replace("-", ""));

            HMACMD5 secondhash = new HMACMD5(temp_key);
            byte[] nonce_hmac_key = secondhash.ComputeHash(SaltedNonce);
            Console.WriteLine("SIGCHK: nonce_hmac_key -> 0x" + BitConverter.ToString(temp_key).Replace("-", ""));

            HMACSHA1 thirdhash = new HMACSHA1(nonce_hmac_key);
            byte[] test_signature = thirdhash.ComputeHash(PA_XBOX_CLIENT_VERSION.Version);
            Console.WriteLine("SIGCHK: test_signature -> 0x" + BitConverter.ToString(test_signature).Replace("-", ""));

            Console.WriteLine("SIGCHK: client_signature -> 0x" + BitConverter.ToString(PA_XBOX_CLIENT_VERSION.Signature).Replace("-", ""));

            if (test_signature.SequenceEqual(Signature))
                return true;

            return false;
        }

        public override void Process()
        {
            // Client Initialization
            {
                Client.UniqueID = 1; // Temp

                Client.SerialNumber = CNAME;

                Client.GamerTag = "SN." + Client.SerialNumber;

                Client.Realm = "PASSPORT.NET";
                Client.Domain = "XBOX.COM";

                Client.Key = OnlineKey;

                Client.Till = TILL;
                Client.Nonce = NONCE;
            }

            var XCLIENTCHK = VerifyXClient(KDC_OPTIONS, REALM, SNAME1, SNAME2, ENC_TYPE);

            if (XCLIENTCHK)
            {
                Console.WriteLine("AS-REQ: XCLIENTCHK -> " + XCLIENTCHK);

                var SIGCHK = SignatureCheckXClient(OnlineKey, PA_XBOX_CLIENT_VERSION.Signature);

                if (SIGCHK)
                {
                    Console.WriteLine("AS-REQ: SIGCHK -> " + SIGCHK);

                    var TSCHK = VerifyTSXClient(OnlineKey, PA_ENC_TIMESTAMP.Timestamp);

                    if (TSCHK)
                    {
                        Console.WriteLine("AS-REQ: TSCHK -> " + TSCHK);

                        BuildResponse();
                    }
                }
            }
            else
            {
                // Drop client XD
                // Bye!
            }
        }

        public void BuildResponse()
        {
            AsnElt accountInfo = new PA_DATA().Encode203(1, Client.GamerTag, Client.Domain, Client.Realm, Client.Key);

            List<string> cnames = new List<string>()
            {
                Client.SerialNumber,
                Client.Realm
            };

            List<AsnElt> allNodes = new List<AsnElt>();

            // Header 

            AsnElt pvnoASN = AsnElt.MakeInteger(5);
            AsnElt pvnoSEQ = AsnElt.Make(AsnElt.SEQUENCE, pvnoASN);
            pvnoSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, pvnoSEQ);
            allNodes.Add(pvnoSEQ);

            AsnElt msg_typeASN = AsnElt.MakeInteger(11);
            AsnElt msg_typeSEQ = AsnElt.Make(AsnElt.SEQUENCE, msg_typeASN);
            msg_typeSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, msg_typeSEQ);
            allNodes.Add(msg_typeSEQ);

            // End


            // Machine Account Info PA_DATA

            AsnElt typeElt = AsnElt.MakeInteger(203);
            AsnElt nameTypeSeq = AsnElt.Make(AsnElt.SEQUENCE, typeElt);
            nameTypeSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, nameTypeSeq);

            AsnElt padataSeq = AsnElt.Make(AsnElt.SEQUENCE, nameTypeSeq, accountInfo);
            allNodes.Add(padataSeq);

            // End


            // crealm

            crealm = new PrincipalName(Client.Realm);

            AsnElt crealmElt = crealm.Encode();
            crealmElt = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, crealmElt);
            allNodes.Add(crealmElt);

            // End


            // cname

            cname = new PrincipalName(cnames);

            AsnElt cnameElt = cname.Encode();
            cnameElt = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, cnameElt);
            allNodes.Add(cnameElt);

            // End

            // ticket

            reqTicket = new Ticket();
            AsnElt ticketElt = reqTicket.Encode(OnlineKey);
            ticketElt = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, ticketElt);
            allNodes.Add(ticketElt);

            // End

            AsnElt seq = AsnElt.Make(AsnElt.SEQUENCE, allNodes.ToArray());

            byte[] toSend = seq.Encode();

            Console.WriteLine("AS-REQ: Response -> " + BitConverter.ToString(toSend).Replace("-", ""));

            this.Client.Send(toSend);
        }

        public override string ToString()
        {
            return "AS_REQ";
        }
    }
}
