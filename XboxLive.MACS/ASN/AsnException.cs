using System;
using System.IO;

namespace XboxLive.MACS.ASN
{
    public class AsnException : IOException
    {
        public AsnException(string message)
            : base(message)
        {
        }

        public AsnException(string message, Exception nested)
            : base(message, nested)
        {
        }
    }
}