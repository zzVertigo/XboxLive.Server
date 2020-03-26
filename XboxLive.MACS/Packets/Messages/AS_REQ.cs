using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NLog;
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
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();
<<<<<<< HEAD

        // == TEMPORARY HARDCODED VALUES ==
        // All of these are 16 bytes, and only work with the default XQEMU EEPROM values
=======
>>>>>>> 2acb7475c9722854e79c2a3d6f856e7ff50e085a

        // Decrypted Client Online Key - TODO: Decrypt this from the client
        public byte[] OnlineKey =
        {
            0x4f, 0xf6, 0xea, 0xa3, 0x86, 0x08, 0xdd, 0xc5, 0x95, 0x08, 0x55, 0xbf, 0xee, 0xc7, 0xdd, 0x00
        };

        // Random Session Key - TODO: Figure out if this is randomly generated or created through client info
        public byte[] SessionKey =
        {
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06
        };

        public byte[] SaltedNonce =
        {
            0x5F, 0xF3, 0x28, 0x92, 0x13, 0x8C, 0x9C, 0x4B, 0x05, 0x84, 0x9A, 0x3C, 0x10, 0x1A, 0xDB, 0x5D
        };

        public AS_REQ(XClient client) : base(client)
        {
        }

        public PrincipalName cname { get; set; }
        public PrincipalName crealm { get; set; }
        public Ticket reqTicket { get; set; }
        public EncKDCRepPart EndPart { get; set; }
        public byte[] nonceHmac { get; set; }

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
            var dec_ts = KerberosCrypto.KerberosDecrypt(Interop.KERB_ETYPE.rc4_hmac,
                Interop.KRB_KEY_USAGE_AS_REQ_PA_ENC_TIMESTAMP, OnlineKey, EncryptedTS);

            var timestamp_encoded = AsnElt.Decode(dec_ts, false);

            var timestamp = timestamp_encoded.Sub[0].Sub[0].GetTime(24);
            var usec = timestamp_encoded.Sub[1].Sub[0].GetInteger();

            if (timestamp != null && usec != null)
            {
                Logger.Info("Successfully decrypted & decoded timestamp/usec");
                Logger.Info(timestamp + " : " + usec);

                return true;
            }

            return false;
        }

        public bool SignatureCheckXClient(byte[] OnlineKey, byte[] Signature)
        {
            var firsthash = new HMACMD5(OnlineKey);
            var temp_key = firsthash.ComputeHash(Encoding.UTF8.GetBytes("signaturekey\0"));
            Logger.Info("temp_key -> 0x" + BitConverter.ToString(temp_key).Replace("-", ""));

            var secondhash = new HMACMD5(temp_key);
            nonceHmac = secondhash.ComputeHash(SaltedNonce);
            Logger.Info("nonce_hmac_key -> 0x" + BitConverter.ToString(temp_key).Replace("-", ""));

            Logger.Info("Version String: " + BitConverter.ToString(PA_XBOX_CLIENT_VERSION.Version));

            var thirdhash = new HMACSHA1(nonceHmac);
            var test_signature = thirdhash.ComputeHash(PA_XBOX_CLIENT_VERSION.Version);
            Logger.Info("SIGCHK: test_signature -> 0x" + BitConverter.ToString(test_signature).Replace("-", ""));

            Logger.Info("SIGCHK: client_signature -> 0x" +
                        BitConverter.ToString(PA_XBOX_CLIENT_VERSION.Signature).Replace("-", ""));

            if (test_signature.SequenceEqual(Signature))
                return true;

            Logger.Warn("Failed to signature check XClient!");

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
                Logger.Info("XCLIENTCHK -> " + XCLIENTCHK);

                var SIGCHK = SignatureCheckXClient(OnlineKey, PA_XBOX_CLIENT_VERSION.Signature);

                if (SIGCHK)
                {
                    Logger.Info("SIGCHK -> " + SIGCHK);

                    var TSCHK = VerifyTSXClient(OnlineKey, PA_ENC_TIMESTAMP.Timestamp);

                    if (TSCHK)
                    {
                        Logger.Info("TSCHK -> " + TSCHK);

                        BuildResponse();
                    }
                }
            }
        }

        public void BuildResponse()
        {
            // Possible suspects as to why the Xbox won't accept AS_REP
            // - cname/sname
            // - ticket
            // - enckdcpart

            // TODO: Find out what the MD4 hashed key is.
            var accountInfo = new PA_DATA().Encode203(1, Client.GamerTag, Client.Domain, Client.Realm,
                Encoding.UTF8.GetBytes(""));

            var cnames = new List<string>
            {
                Client.SerialNumber,
                Client.Realm
            };

            var allNodes = new List<AsnElt>();

            // Header 

            var pvnoASN = AsnElt.MakeInteger(5);
            var pvnoSEQ = AsnElt.Make(AsnElt.SEQUENCE, pvnoASN);
            pvnoSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 0, pvnoSEQ);
            allNodes.Add(pvnoSEQ);

            var msg_typeASN = AsnElt.MakeInteger(11);
            var msg_typeSEQ = AsnElt.Make(AsnElt.SEQUENCE, msg_typeASN);
            msg_typeSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, msg_typeSEQ);
            allNodes.Add(msg_typeSEQ);

            // End

            // Machine Account Info PA_DATA

            var encryptedAccount = new EncryptedData((int) Interop.KERB_ETYPE.rc4_hmac, 1,
                KerberosCrypto.KerberosEncrypt(Interop.KERB_ETYPE.rc4_hmac,
                    Interop.KRB_KEY_USAGE_KRB_PRIV_ENCRYPTED_PART, nonceHmac, accountInfo.Encode()));

            var typeElt = AsnElt.MakeInteger(203);
            var nameTypeSeq = AsnElt.Make(AsnElt.SEQUENCE, typeElt);
            nameTypeSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 2, nameTypeSeq);

            var padataSeq = AsnElt.Make(AsnElt.SEQUENCE, nameTypeSeq, encryptedAccount.Encode());
            //allNodes.Add(padataSeq);

            // End

            // crealm

            var crealmElt = AsnElt.MakeString(AsnElt.GeneralString, Client.Realm);
            var crealmSeq = AsnElt.Make(AsnElt.SEQUENCE, crealmElt);
            crealmSeq = AsnElt.MakeImplicit(AsnElt.CONTEXT, 3, crealmSeq);
            allNodes.Add(crealmSeq);

            // End

            // cname

            cname = new PrincipalName(cnames, 2);

            var cnameElt = cname.Encode();
            cnameElt = AsnElt.MakeImplicit(AsnElt.CONTEXT, 4, cnameElt);
            allNodes.Add(cnameElt);

            // End

            // ticket

            reqTicket = new Ticket();
            var ticketElt = reqTicket.Encode(OnlineKey);
            ticketElt = AsnElt.MakeImplicit(AsnElt.CONTEXT, 5, ticketElt);
            allNodes.Add(ticketElt);

            // End

            // enckdcpart

            EndPart = new EncKDCRepPart();
            {
                // Used to send the online key, now we're sending an arbitrary session key
                EndPart.key = new EncryptionKey();
                {
<<<<<<< HEAD
                    EndPart.key.keytype = (int)Interop.KERB_ETYPE.rc4_hmac;
                    EndPart.key.keyvalue = SessionKey;
=======
                    EndPart.key.keytype = (int) Interop.KERB_ETYPE.rc4_hmac;
                    EndPart.key.keyvalue = OnlineKey; // fill it with 0's :P
>>>>>>> 2acb7475c9722854e79c2a3d6f856e7ff50e085a
                }

                EndPart.lastReq = new LastReq();
                {
                    // 0 - no info
                    // 1 - last intial TGT request
                    // 2 - last intial request
                    // 3 - newest TGT used
                    // 4 - last renewal
                    // 5 - last request (of any type)

                    EndPart.lastReq.lr_type = 5;
                    EndPart.lastReq.lr_value = DateTime.Now;
                }

                EndPart.nonce = (uint) new Random(1206).Next(1000, 10000);

                EndPart.flags = Interop.TicketFlags.initial | Interop.TicketFlags.pre_authent;

                EndPart.authtime = DateTime.Now;

                EndPart.endtime = new DateTime(2019, 8, 7);

                EndPart.realm = "MACS.XBOX.COM";
            }

            // TODO: Move encryption to EncryptedData class
            var EndPartData = EndPart.Encode().Encode();

            EndPartData = KerberosCrypto.KerberosEncrypt(Interop.KERB_ETYPE.rc4_hmac,
                Interop.KRB_KEY_USAGE_AS_REP_EP_SESSION_KEY, Client.Key, EndPartData);

            var encData = new EncryptedData((int) Interop.KERB_ETYPE.rc4_hmac, 1, EndPartData);

            var encPart = AsnElt.MakeImplicit(AsnElt.CONTEXT, 6, encData.Encode());
            allNodes.Add(encPart);

            // End

            var seq = AsnElt.Make(AsnElt.SEQUENCE, allNodes.ToArray());
            var seq2 = AsnElt.Make(AsnElt.SEQUENCE, seq);
            seq2 = AsnElt.MakeImplicit(AsnElt.APPLICATION, 11, seq2);

            var toSend = seq2.Encode();

            Logger.Info(BitConverter.ToString(toSend).Replace("-", ""));

            Client.Send(toSend);
        }

        public override string ToString()
        {
            return "AS_REQ";
        }
    }
}