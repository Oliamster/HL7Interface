using HL7Interface;
using HL7Interface.ServerProtocol;
using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7DeviceSimulator.Command
{
    internal class V251EquipmentCommandRequestMLLP : CommandBase<HL7Session, HL7Request>
    {
        public override void ExecuteCommand(HL7Session session, HL7Request requestInfo)
        {
            throw new NotImplementedException();
        }
    }
}
