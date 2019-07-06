using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using NHapiTools.Base.Util;
using HL7Interface.Configuration;
using HL7Interface.ServerProtocol;
using HL7api.Parser;
using HL7api.Model;

namespace Hl7Interface.ServerProtocol
{
    public class HL7ProtocolBase : IHL7Protocol
    {
        #region Private Properties
        private IProtocolConfig config;
        private HL7Parser p = new HL7Parser();
        #endregion

        #region Constructors
        public HL7ProtocolBase()
        {
        }

        public HL7ProtocolBase(IProtocolConfig Config)
        {
            this.Config = Config;
        }
        #endregion

        #region  Public Properties
        public IProtocolConfig Config { get; set; } 
        #endregion

        #region IProtocol Interface
        public virtual byte[] Encode(IHL7Message hl7Message)
        {
            return  Encoding.UTF8.GetBytes(MLLP.CreateMLLPMessage(hl7Message.Encode()));
        }

        public virtual ParserResult Parse(string message)
        {
            return p.Parse(message);
        } 
        #endregion
    }
}
