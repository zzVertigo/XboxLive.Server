using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using XboxLive.MACS.ASN;
using XboxLive.MACS.Core;
using XboxLive.MACS.Structures;
using XboxLive.MACS.Structures.KRB_Structures;
using XboxLive.MACS.Structures.PA_Structures;

namespace XboxLive.MACS.Packets.Messages.Server
{
    public class AS_REP : Message
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public AsnElt accountInfo;

        public AS_REP(XClient client) : base(client)
        {
            MSG_TYPE = 11;
            this.Client = client;
        }

        public override byte[] Encode()
        {
            accountInfo = new PA_XBOX_ACCOUNT_CREATION().Encode(1, Client.GamerTag, Client.Domain, Client.Realm, Client.NonceHmacKey);

            AsnElt pvnoASN = AsnElt.MakeInteger(5);
            AsnElt pvnoSEQ = AsnElt.Make(AsnElt.SEQUENCE, new[] {pvnoASN});
            pvnoSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 0, pvnoSEQ);

            AsnElt msgtypeASN = AsnElt.MakeInteger(11);
            AsnElt msgtypeSEQ = AsnElt.Make(AsnElt.SEQUENCE, new[] {msgtypeASN});
            msgtypeSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 1, msgtypeSEQ);

            AsnElt padataASN = AsnElt.Make(AsnElt.SEQUENCE, new[] { accountInfo });
            AsnElt padataSEQ = AsnElt.Make(AsnElt.CONTEXT, new[] { padataASN });
            padataSEQ = AsnElt.MakeImplicit(AsnElt.CONTEXT, 2, padataSEQ);

            AsnElt[] total = new[] {pvnoSEQ, msgtypeSEQ, padataSEQ};
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
                        Logger.Info("PVNO: " + s.Sub[0].GetInteger());
                        break;

                    case 1: // msg_type
                        Logger.Info("MSG_TYPE: " + s.Sub[0].GetInteger());
                        break;

                    //case 2: // pa_data
                    //    break;

                    //case 3: // crealm
                    //    break;

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
