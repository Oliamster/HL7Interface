using HL7Interface.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Interface.ServerProtocol
{
    public interface IProtocol
    {
        IProtocolConfig Config { get; set; }
    }
}
