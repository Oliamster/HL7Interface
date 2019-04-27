using HL7api.V251.Message;
using HL7Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationManager
{
    class DeviceInterface : BaseHL7Interface
    {
        public DeviceInterface()
        {
        }

        public Task<HL7Request> PrepareForAcquisition()
        {
            return SendHL7MessageAsync(new EquipmentCommandRequest());
        }
    }
}
