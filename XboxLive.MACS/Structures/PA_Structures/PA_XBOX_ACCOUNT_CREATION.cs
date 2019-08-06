using System;
using System.Collections.Generic;
using System.Text;

namespace XboxLive.MACS.Structures.PA_Structures
{
    public class PA_XBOX_ACCOUNT_CREATION
    {
        public long PUID { get; set; } // 1 (ex)
        public string GamerTag { get; set; } // SN.XXXXXXXXXXXX (Console Serial Number)
        public string Domain { get; set; } // xbox.com
        public string Realm { get; set; } // passport.net
        public byte[] Key { get; set; } // MD4 hashed key, not the password
    }
}
