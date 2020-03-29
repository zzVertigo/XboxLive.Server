using NLog;
using System;
using System.Collections.Generic;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Core;
using XboxLive.MACS.Crypto;
using XboxLive.MACS.Structures.KRB_Structures;
using XboxLive.MACS.Structures.PA_Structures;

namespace XboxLive.MACS.Packets.Messages.Server
{
    //AS-REP          ::= [APPLICATION 11] KDC-REP

    //KDC-REP         ::= SEQUENCE {
    //        pvno            [0] INTEGER (5), // done
    //        msg-type        [1] INTEGER (11 -- AS), // done
    //        padata          [2] SEQUENCE OF PA-DATA OPTIONAL // def done
    //                                -- NOTE: not empty --,
    //        crealm          [3] Realm, // done (maybe)
    //        cname           [4] PrincipalName, // done
    //        ticket          [5] Ticket, // done (maybe)
    //        enc-part        [6] EncryptedData // done (NOT EVEN SURE)
    //                                -- EncASRepPart
    //}

    public class AS_REP : Message
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public AsnElt accountInfo;
        public PrincipalName cname;

        public Ticket ticket;

        public EncryptedData encpart;

        // I almost feel like I can choose what the TGS secret key is since I am in the position of the server but its like how would the Xbox know how to decrypt the ticket its so WEIRD

        // Random Session Key - this is the shared key between TGS and client..? should be pre random but static will do
        // Also shares the same length as the online key which is 16 bytes
        public byte[] SessionKey =
        {
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06
        };

        public byte[] TgsSecret =
        {
            0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06
        };

        public AS_REP(XClient client) : base(client)
        {
            this.MSG_TYPE = 11;
            this.Client = client;

            // suffix should be @MACS.XBOX.COM but still not 100% sure about that one
            List<string> cnames = new List<string>();
            {
                cnames.Add(this.Client.GamerTag);
                cnames.Add("PASSPORT.NET");
            }

            this.cname = new PrincipalName(cnames, 2);

            List<string> snames = new List<string>(2);
            {
                snames.Add("krbtgt");
                snames.Add("MACS.XBOX.COM");
            }

            // supposed to be encrypted with some TGS secret key lol
            this.ticket = new Ticket();
            {
                this.ticket.tkt_vno = 5;
                this.ticket.realm = "PASSPORT.NET";
                this.ticket.sname = new PrincipalName(snames, 2);
                this.ticket.encdata = new EncryptedData((int)Interop.KERB_ETYPE.rc4_hmac, 1, this.TgsSecret);
            }

            // Basically an encrypted form of the session key using the users secret key (aka the online key)
            byte[] encrypted_online_key = KerberosCrypto.KerberosEncrypt(Interop.KERB_ETYPE.rc4_hmac,
                Interop.KRB_KEY_USAGE_AS_REP_EP_SESSION_KEY, this.Client.OnlineKey, this.SessionKey);

            this.encpart = new EncryptedData((int)Interop.KERB_ETYPE.rc4_hmac, 1, encrypted_online_key);

            this.accountInfo = new PA_XBOX_ACCOUNT_CREATION().Encode(1, Client.GamerTag, Client.Domain, Client.Realm, Client.NonceHmacKey);
        }

        public override byte[] Encode()
        {
            AsnElt pvnoASN = AsnElt.MakeInteger(this.PVNO);
            AsnElt pvnoSEQ = AsnElt.Make(AsnElt.SEQUENCE, pvnoASN);
            pvnoSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 0, pvnoSEQ);

            AsnElt msgtypeASN = AsnElt.MakeInteger(11);
            AsnElt msgtypeSEQ = AsnElt.Make(AsnElt.SEQUENCE, msgtypeASN);
            msgtypeSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, msgtypeSEQ);

            AsnElt padataASN = AsnElt.Make(AsnElt.SEQUENCE, accountInfo);
            AsnElt padataSEQ = AsnElt.Make(AsnElt.CONTEXT, padataASN);
            padataSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 2, padataSEQ);

            AsnElt crealmASN = AsnElt.MakeString(AsnElt.IA5String, "PASSPORT.NET");
            crealmASN = AsnElt.MakeImplicit(AsnElt.UNIVERSAL, AsnElt.GeneralString, crealmASN);
            AsnElt crealmSEQ = AsnElt.Make(AsnElt.SEQUENCE, crealmASN);
            crealmSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 3, crealmSEQ);

            AsnElt cnameELT = this.cname.Encode();
            cnameELT = AsnElt.MakeImplicit(AsnElt.CONTEXT, 4, cnameELT);

            AsnElt ticketASN = this.ticket.Encode();
            AsnElt ticketSEQ = AsnElt.Make(AsnElt.SEQUENCE, ticketASN);
            ticketSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 5, ticketSEQ);

            AsnElt encpartASN = this.encpart.Encode();
            AsnElt encpartSEQ = AsnElt.Make(AsnElt.SEQUENCE, encpartASN);
            encpartSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 6, encpartSEQ);

            AsnElt[] total = new[] {pvnoSEQ, msgtypeSEQ, padataSEQ, crealmSEQ, cnameELT, ticketSEQ, encpartSEQ };
            var seq = AsnElt.Make(AsnElt.SEQUENCE, total);

            AsnElt totalSeq = AsnElt.Make(AsnElt.SEQUENCE, new [] {seq});
            totalSeq = AsnElt.MakeImplicit(AsnElt.APPLICATION, 11, totalSeq);

            byte[] toSend = totalSeq.Encode();

            Logger.Info("Data: " + BitConverter.ToString(toSend).Replace("-", ""));

            Logger.Info(testDecode(toSend) ? "Test decode success!" : "Test decode failed!");

            return toSend;
        }

        private bool testDecode(byte[] data)
        {
            AsnElt AS_REP = AsnElt.Decode(data, false);


            if (AS_REP.TagValue != 11)
            {
                Logger.Warn("AS-REP tag value should be 11!");
            }

            if ((AS_REP.Sub.Length != 1) || (AS_REP.Sub[0].TagValue != 16))
            {
                Logger.Warn("First AS-REP sub should be a sequence!");
            }

            AsnElt[] kdc_rep = AS_REP.Sub[0].Sub;

            foreach (AsnElt s in kdc_rep)
            {
                switch (s.TagValue)
                {
                    case 0: // pvno
                        Logger.Info("Valid PVNO");
                        break;

                    case 1: // msg_type
                        Logger.Info("Valid MSG_TYPE");
                        break;

                    case 2: // pa_data
                        Logger.Info("Valid PA_DATA");
                        break;

                    case 3: // crealm
                        Logger.Info("Valid CREALM");
                        break;

                    //case 4: // cname
                    //    Logger.Info("CNAME: " + new PrincipalName(s.Sub[0]));
                    //    break;

                    //case 5: // ticket
                    //    Logger.Info("TICKET: " + new Ticket());
                    //    break;

                    //case 6: // enc_part
                    //    break;

                    default:
                        break;
                }
            }

            if (kdc_rep != null)
                return true;

            return false;
        }

        public override string ToString()
        {
            return "AS_REP";
        }
    }
}
