using NHapi.Base.Model;
using HL7api.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHapi.Base.Util;

namespace HL7api.Model
{
    public abstract class AbstractHL7Message : IHL7Message
    {
        protected IMessage m_Message;
        private HL7Parser hl7Parser;
        protected Terser terser;
   

        public AbstractHL7Message(IMessage message)
        {
            this.m_Message = message;
            this.hl7Parser = new HL7Parser();
            this.terser = new Terser(m_Message);
        }
        public string MessageName => this.GetType().Name; //Change to RequestKey

        public abstract  DateTime MessageDateTime { get; }

        public virtual string MessageID => throw new NotImplementedException();

        public string MessageVersion => this.m_Message.Version;

        public virtual string ExpectedResponseName => throw new NotImplementedException();

        public TransactionType TypeOfTransaction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public virtual string Trigger => throw new NotImplementedException();

        public virtual string Code => throw new NotImplementedException();

        public IMessage Message => this.m_Message;

        public string Encode()
        {
            return hl7Parser.Encode(this);
        }
        public string Encode(HL7Encoding hL7Encoding)
        {
            return hl7Parser.Encode(this, hL7Encoding, true);
        }

        public string GetValue(string path)
        {
            return terser.Get(path);
        }

        public void SetValue(string path, string newValue)
        {
            terser.Set(path, newValue);
        }

       
    }
}
