using HL7api.V251.Message;
using HL7Interface.Configuration;
using HL7Interface.ServerProtocol;
using NHapiTools.Base.Util;
using SuperSocket.SocketBase.Command;
using System.Text;

namespace HL7Interface
{
    public class V251EquipmentCommandRequest : CommandBase<HL7Session, HL7Request>
    {
        public override void ExecuteCommand(HL7Session session, HL7Request packageInfo)
        {
            EquipmentCommandResponse response = ExecuteCommand(packageInfo);
            byte[] bytesToSend = Encoding.UTF8.GetBytes(MLLP.CreateMLLPMessage(response.Encode()));

            HL7SocketServiceConfig config = session.AppServer.Config as HL7SocketServiceConfig;
            
            //config.ProtocolConfig

            session.Send(bytesToSend, 0, bytesToSend.Length);
        }

        protected virtual EquipmentCommandResponse ExecuteCommand(HL7Request request)
        {
            return new EquipmentCommandResponse();
        }

        public override string Name => this.GetType().Name;
    }
}
