using SuperSocket.SocketBase.Config;
using SuperSocket.SocketEngine.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
