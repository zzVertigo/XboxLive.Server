using System;
using System.Collections.Generic;
using XboxLive.MACS.Packets.Messages;

namespace XboxLive.MACS.Packets
{
    public class MessageFactory
    {
        public static Dictionary<int, Type> Messages = new Dictionary<int, Type>();

        public MessageFactory()
        {
            LoadMessages();
        }

        private static void LoadMessages()
        {
            Messages.Add(10, typeof(AS_REQ));
        }
    }
}