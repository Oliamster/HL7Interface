using NHapi.Base.Model;
using HL7api.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7api.Model
{
    public abstract class AbstractHL7Message : IHL7Message
    {
        protected IMessage m_Message;
        private HL7Parser hl7Parser;

        public AbstractHL7Message(IMessage message)
        {
            this.m_Message = message;
            this.hl7Parser = new HL7Parser();
        }
        public string MessageName => this.GetType().Name;

        public abstract  DateTime MessageDateTime { get; }

        public virtual string MessageID => throw new NotImplementedException();

        public string MessageVersion => this.m_Message.Version;

        public string ExpectedResponseName => throw new NotImplementedException();

        public TransactionType TypeOfTransaction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public virtual string Trigger => throw new NotImplementedException();

        public virtual string Code => throw new NotImplementedException();

        public IMessage Message => this.m_Message;

        public string Encode()
        {
            return hl7Parser.Encode(this);
        }
    }
}
