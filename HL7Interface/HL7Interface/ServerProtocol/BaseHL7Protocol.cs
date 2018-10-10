using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using HL7Interface.Configuration;
using HL7Interface.ServerProtocol;
using HL7api.Parser;
using HL7api.Model;

namespace Hl7Interface.ServerProtocol
{
    public class BaseHL7Protocol : IHL7Protocol
    {
        private IProtocolConfig config;
        private HL7Parser p = new HL7Parser();


        public IProtocolConfig Config { get; set; }

       

        #region IProtocol Interface
        public virtual byte[] Encode(IHL7Message hl7Message)
        {
            ////string mllpMessage = HL7api.Util.MLLP.CreateMLLPMessage(hl7Message.Encode());
            //return Encoding.ASCII.GetBytes(mllpMessage);
            throw new NotImplementedException();
        }

        public virtual ParserResult Parse(string message)
        {
            message = Encoding.ASCII.GetString(Convert.FromBase64String(message));
            
            //return p.Parse(message);
            throw new NotImplementedException();
        } 
        #endregion
    }
}
