using HL7Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7DeviceSimulator.Command
{
    class PrepareCommand : V251EquipmentCommandRequest
    {
        protected override global::HL7api.V251.Message.EquipmentCommandResponse ExecuteCommand(HL7Request request)
        {
            return base.ExecuteCommand(request);
        }
    }
}
