using NHapi.Base.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHapiPlus.Model
{
    public abstract class AbstractHL7Message : IHL7Message
    {
        protected IMessage m_Message;
        public AbstractHL7Message(IMessage message)
        {
            this.m_Message = message;
        }
        public string MessageName => this.GetType().Name;

        public abstract  DateTime MessageDateTime { get; }
       

        public string MessageID => throw new NotImplementedException();

        public string MessageVersion => throw new NotImplementedException();

        public string ExpectedResponseName => throw new NotImplementedException();

        public TransactionType TypeOfTransaction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
