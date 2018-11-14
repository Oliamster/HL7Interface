using HL7api.Model;
using SuperSocket.SocketBase.Protocol;

namespace HL7Interface
{
    public class HL7Request : IRequestInfo
    {
        public HL7Request()
        {

        }

        public string Key { get; set; }

        public IHL7Message RequestMessage { get; set; }

        public IHL7Message Acknowledgment { get; set; }
    }
}