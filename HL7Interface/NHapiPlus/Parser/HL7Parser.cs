using NHapiPlus.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHapiPlus.Parser
{
    public class HL7Parser
    {
        public IHL7Message Parser(string message, bool validate)
        {
            throw new NotImplementedException();
        }

        public string Encode (IHL7Message hl7Message, HL7Encoding encoding, bool validate)
        {
            throw new NotImplementedException();
        }
    }
}
