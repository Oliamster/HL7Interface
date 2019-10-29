using HL7api.Model;
using SuperSocket.SocketBase.Protocol;
using System.Threading;
using System.Threading.Tasks;

namespace HL7Interface
{
    public class HL7Request : IRequestInfo
    {
        public string Key
        {
            get; set;
        }

        public IHL7Message Request
        {
            get; set;
        }

        public IHL7Message Acknowledgment
        {
            get; set;
        }

        public IHL7Message Response
        {
            get; set;
        }
        
        internal AutoResetEvent ResponseReceivedEvent
        {
            get; set;
        }

        internal AutoResetEvent AckReceivedEvent
        {
            get; set;
        }

        internal Task<HL7Request> SenderTask
        {
            get;
            set;
        }

        internal AutoResetEvent RequestCompletedEvent
        {
            get; set;
        }

        internal CancellationTokenSource RequestCancellationToken
        {
            get; set;
        }
    }
}