using System;
using HL7api.Parser;
using NHapi.Base.Util;
using NHapi.Base.Model;
using NHapi.Base;

namespace HL7api.Model
{
    /// <summary>
    /// The abstact HL7 MEssage
    /// </summary>
    public abstract class HL7Message : IHL7Message 
    {
        protected IMessage m_Message; 
        private HL7Parser m_HL7Parser;
        protected Terser m_Terser;
        private ISegment m_MSH;

        /// <summary>
        /// Create a new instance of HL7Message
        /// </summary>
        /// <param name="message"></param>
        public HL7Message(IMessage message)
        {
            this.m_Message = message;
            this.m_HL7Parser = new HL7Parser();
            this.m_Terser = new Terser(m_Message);
            this.m_MSH = m_Terser.getSegment("MSH");
        }

        public string MessageID
        {
            get { return GetType().Name; }
        }

        public abstract DateTime MessageDateTime { get; }

        public virtual string ControlID => Terser.Get(m_MSH, 10,  0, 1, 1);

        public string HL7Version => this.m_Message.Version;

        public virtual string ExpectedResponseID => throw new NotImplementedException();

        public TransactionType TypeOfTransaction { get; set; }

        public virtual string Trigger => Terser.Get(m_MSH, 9, 0, 2, 1);

        public virtual string Code => Terser.Get(m_MSH, 9, 0, 1, 1);

        /// <summary>
        /// The IMessage Interface
        /// </summary>
        public IMessage Message => this.m_Message;

        public abstract string ExpectedAckID { get; }

        public string Encode()
        {
            return m_HL7Parser.Encode(this);
        }
        public string Encode(HL7Encoding hL7Encoding)
        {
            return m_HL7Parser.Encode(this, hL7Encoding, true);
        }

        public abstract bool IsAcknowledge { get; }
        public abstract string ExpectedResponseType { get; }
        public abstract string ExpectedAckType { get; }

        public string GetValue(string path)
        {
            try
            {
                return m_Terser.Get(path);
            }
            catch (HL7Exception he)
            {
                throw new HL7apiException($"Unable to get value form path: {path}", he);
            }
        }

        public void SetValue(string path, string newValue)
        {
            try
            {
                m_Terser.Set(path, newValue);
            }
            catch (HL7Exception he)
            {
                throw new HL7apiException($"Unable to set the value to loction: {path}", he);
            }
        }
        public abstract bool IsResponseForRequest(IHL7Message request);

        public virtual bool IsAckForRequest(IHL7Message request)
        {
            if (!this.IsAcknowledge)
                return false;

            return HL7Parser.IsAckForRequest(request, this);
        }
    }
}
