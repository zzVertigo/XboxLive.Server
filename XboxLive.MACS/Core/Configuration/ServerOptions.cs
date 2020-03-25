namespace XboxLive.MACS.Core.Configuration
{
    public class ServerOptions
    {
        public string Address { get; set; } // Address of the server
        public int Port { get; set; } // Port the server will listen on
        public bool Debug { get; set; } // Whether debug logging is enabled or not
    }
}