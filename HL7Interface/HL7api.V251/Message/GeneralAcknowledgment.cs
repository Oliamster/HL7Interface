﻿using NHapi.Model.V251.Message;
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


        public override string ControlID
        {
            get
            {
                string ret = null;
                ret = ack.MSH.MessageControlID.Value;
                return ret;
            }
        }


        public override string Trigger => ack.MSH.MessageType.TriggerEvent.Value;

        public override string Code => ack.MSH.MessageType.MessageCode.Value;


        public override bool IsAcknowledge => true;
        public override DateTime MessageDateTime => throw new NotImplementedException();

        public override string ExpectedAckID => null;

        public override string ExpectedResponseID => default(string);

        public override string ExpectedResponseType => String.Empty;

        public override string ExpectedAckType => String.Empty;

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
