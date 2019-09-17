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
        private HL7Parser m_HL7Parser = new HL7Parser();
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
            string mllpMessage = MLLP.CreateMLLPMessage(hl7Message.Encode());

            byte[] bytesMessge = Encoding.ASCII.GetBytes(mllpMessage);

            //string base64Message = Convert.ToBase64String(bytesMessge);

            return Encoding.ASCII.GetBytes(mllpMessage);
        }

        public virtual ParserResult Parse(byte[] messageBytes)
        {
            string messageBase64 = Encoding.ASCII.GetString(messageBytes);

            //byte[] raw = Convert.FromBase64String(messageBase64);

            //string messageToParse = Encoding.ASCII.GetString(raw);

            HL7api.Util.MLLP.StripMLLPContainer(ref messageBase64);

            return m_HL7Parser.Parse(messageBase64);
        }
        #endregion
    }
}
