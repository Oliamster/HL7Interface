using SuperSocket.SocketEngine.Configuration;
using System.Configuration;

namespace HL7Interface.Configuration
{
    public class HL7SocketServiceConfig : SocketServiceConfig
    {
        [ConfigurationProperty("protocolConfig")]
        public ProtocolConfig ProtocolConfig
        {
            get
            {
                return this["protocolConfig"] as ProtocolConfig;
            }
        }
    }
}
