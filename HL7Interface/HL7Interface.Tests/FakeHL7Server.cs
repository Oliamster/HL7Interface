using System;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using HL7Interface.ServerProtocol;

namespace HL7Interface.Tests
{
    /// <summary>
    /// The HL7 MLLP server
    /// </summary>
    public class FakeHL7Server : AppServer<HL7Session, HL7Request>
    {
        public FakeHL7Server()
            : base(new DefaultReceiveFilterFactory<MLLPBeginEndMarkReceiveFilter, HL7Request>())
        {
        }
        
        /// <summary>
        /// Execute the command convayed by the HL7Request no ack
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestInfo"></param>
        protected override void ExecuteCommand(HL7Session session, HL7Request requestInfo)
        {
            try
            {
                Logger.Debug($"Message received: {requestInfo.Request.Encode()}");

                base.ExecuteCommand(session, requestInfo);
                Logger.Debug($"The command {requestInfo.Request.MessageID} has been executed");
            }
            catch (Exception e)
            {
                Logger.Debug($"ERROR: {e.Message}");
                throw e;
            }
        }
    }
}
