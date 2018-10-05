using NHapi.Base.Parser;
using HL7api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHapi.Base.Model;

namespace HL7api.Parser
{
    public class HL7Parser
    {
        private PipeParser pipeParser;
        private XMLParser xmlParser;
        

        public HL7Parser()
        {
            pipeParser = new PipeParser();
            xmlParser = new DefaultXMLParser();
        }
        public IHL7Message Parse(string message, bool validate)
        {
            return DoParse(pipeParser.Parse(message));
        }

        ParserResult Parse(IHL7Message message)
        {
            throw new NotImplementedException();
        }
        internal IHL7Message DoParse(IMessage message)
        {
            if (message == null)
                throw new HL7apiException();
            throw new NotImplementedException();
        }

        public string Encode (IHL7Message hl7Message, HL7Encoding encoding, bool validate)
        {
            if (HL7Encoding.XML == encoding)
                return xmlParser.Encode(hl7Message.Message);
            return pipeParser.Encode(hl7Message.Message);

        }

        public PipeParser PipeParser => this.pipeParser;

        public string Encode(IHL7Message message)
        {
            return Encode(message, HL7Encoding.ER7, true);
        }
    }
}
