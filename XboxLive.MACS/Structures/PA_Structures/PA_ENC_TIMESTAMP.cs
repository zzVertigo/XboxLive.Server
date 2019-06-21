using System;
using System.Collections.Generic;
using System.Text;

namespace XboxLive.MACS.Structures.PA_Structures
{
    public class PA_ENC_TIMESTAMP
    {
        public byte[] Timestamp { get; set; }
        public byte[] USec { get; set; }
    }
}
