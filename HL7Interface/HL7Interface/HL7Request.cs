using HL7api.Model;
using SuperSocket.SocketBase.Protocol;
using System.Threading;
using System.Threading.Tasks;

namespace HL7Interface
{
    public class HL7Request : IRequestInfo
    {
        public HL7Request() { }

        public HL7Request(IHL7Message message)
        {
            Request = message;
            ResponseReceivedEvent = new AutoResetEvent(false);
            AckReceivedEvent = new AutoResetEvent(false);
            RequestCompletedEvent = new AutoResetEvent(false);
            RequestCancellationToken = new CancellationTokenSource();
        }

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