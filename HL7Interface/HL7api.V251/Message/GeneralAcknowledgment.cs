using NHapi.Model.V251.Message;
using HL7api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7api.V251.Message
{
    public class GeneralAcknowledgment : AbstractHL7Message
    {
        protected ACK ack;

        public ACK ACK
        {
            get
            {
                return this.m_Message as ACK;
            }
        }

        public override DateTime MessageDateTime => throw new NotImplementedException();

        public GeneralAcknowledgment() : this(new ACK())
        {

        }

        public GeneralAcknowledgment(ACK ack)
            : base(ack)
        {
            this.ack = this.m_Message as ACK;
        }
    }
}
