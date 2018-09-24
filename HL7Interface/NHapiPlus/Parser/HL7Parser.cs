using NHapi.Base.Parser;
using NHapiPlus.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHapi.Base.Model;

namespace NHapiPlus.Parser
{
    public class HL7Parser
    {
        PipeParser parser;

        public HL7Parser()
        {
            parser = new PipeParser();
        }
        public IHL7Message Parser(string message, bool validate)
        {
            throw new NotImplementedException();
        }

        internal IHL7Message DoParse(IMessage message)
        {
            throw new NotImplementedException();
        }

        public string Encode (IHL7Message hl7Message, HL7Encoding encoding, bool validate)
        {
            throw new NotImplementedException();
        }
    }
}
