using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7DeviceSimulator.StatePattern
{

    
    internal abstract class State
    {
        protected State _initalState = default(State);
        protected State _previousState;
    }
}
