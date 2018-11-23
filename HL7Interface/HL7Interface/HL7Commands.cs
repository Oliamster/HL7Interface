using HL7api.V251.Message;
using HL7Interface.ServerProtocol;
using NHapiTools.Base.Util;
using SuperSocket.ClientEngine.Protocol;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Interface
{
    public class V251EquipmentCommandRequest : CommandBase<HL7Session, HL7Request>
    {
        public override void ExecuteCommand(HL7Session session, HL7Request packageInfo)
        {
            EquipmentCommandResponse response = new EquipmentCommandResponse();
            byte[] bytesToSend = Encoding.UTF8.GetBytes(MLLP.CreateMLLPMessage(response.Encode()));
            session.Send(bytesToSend, 0, bytesToSend.Length);

        }
        public override string Name => this.GetType().Name;
    }
}
