using System.Text;
using HL7Interface.Configuration;
using HL7Interface.ServerProtocol;
using HL7api.Parser;
using HL7api.Model;

namespace Hl7Interface.ServerProtocol
{
    public class HL7ProtocolBase : IHL7Protocol
    {
        private IProtocolConfig config;

        public IProtocolConfig Config { get; set; }

        public HL7ProtocolBase()
        {

        }

        public HL7ProtocolBase(IProtocolConfig p)
        {
            Config = p;
        }

        //public static IHL7Protocol GetMessageProtocol()
        //{
        //    //IHL7Protocol p = null;
        //    //string messageProtocolTypeName = ConfigurationManager.AppSettings["MessageProtocol"];
        //    //if (messageProtocolTypeName == null)
        //    //    p = new DefaultProtocol();
        //    //else
        //    //{
        //    //    try
        //    //    {
        //    //        Type t = Type.GetType(messageProtocolTypeName);
        //    //        p = Activator.CreateInstance(t) as IHL7Protocol;
        //    //    }
        //    //    catch (Exception e)
        //    //    {
        //    //        throw new HL7InterfaceException
        //    //            ("Unable to instantiate the Message Protocol class from the assembly name provided", e);
        //    //    }
        //    //}
        //    //return p;
        //}

        #region IProtocol Interface
        public virtual byte[] Encode(IHL7Message hl7Message)
        {
            string mllpMessage = HL7api.Util.MLLP.CreateMLLPMessage(hl7Message.Encode());
            return Encoding.ASCII.GetBytes(mllpMessage);
        }

        public virtual ParserResult Parse(byte[] raw)
        {
           string message = Encoding.ASCII.GetString(raw);
            HL7api.Util.MLLP.StripMLLPContainer(ref message);
            HL7Parser p = new HL7Parser();
            return p.Parse(message);
        }
        #endregion













        //#region Private Properties
        //private IProtocolConfig config;
        //private HL7Parser m_HL7Parser = new HL7Parser();
        //#endregion

        //#region Constructors
        //public HL7ProtocolBase()
        //{
        //}

        //public HL7ProtocolBase(IProtocolConfig Config)
        //{
        //    this.Config = Config;
        //}
        //#endregion

        //#region  Public Properties
        //public IProtocolConfig Config { get; set; }
        //#endregion

        //#region IProtocol Interface
        //public virtual byte[] Encode(IHL7Message hl7Message)
        //{
        //    string mllpMessage = MLLP.CreateMLLPMessage(hl7Message.Encode());

        //    byte[] bytesMessge = Encoding.ASCII.GetBytes(mllpMessage);

        //    return Encoding.ASCII.GetBytes(mllpMessage);
        //}

        //public virtual ParserResult Parse(byte[] messageBytes)
        //{
        //    string messageBase64 = Encoding.ASCII.GetString(messageBytes);

        //    HL7api.Util.MLLP.StripMLLPContainer(ref messageBase64);

        //    return m_HL7Parser.Parse(messageBase64);
        //}
        //#endregion
    }
}
