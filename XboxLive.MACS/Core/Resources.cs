using XboxLive.MACS.Packets;

namespace XboxLive.MACS.Core
{
    public class Resources
    {
        public static MessageFactory Factory;

        public static void Start()
        {
            Factory = new MessageFactory();
        }
    }
}