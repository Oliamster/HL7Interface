using System;
using HL7api.Model;
using NHapi.Base.Model;
using NHapi.Model.V251.Message;
using NHapiTools.Base.Util;

namespace HL7api.V251.Message
{
    public class GeneralAcknowledgment : HL7Message, IHL7Acknowledge
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

        internal GeneralAcknowledgment(IMessage message) : base(message) 
        {
            ack = message as ACK;
        }


    

        public override string TriggerEvent => ack.MSH.MessageType.TriggerEvent.Value;

        public override string MessageCode => ack.MSH.MessageType.MessageCode.Value;


        public override bool IsAcknowledge => true;
        public override DateTime MessageDateTime => throw new NotImplementedException();

        public override string ExpectedAckID => string.Empty;

        public override string ExpectedResponseID => string.Empty;

        public override string ExpectedResponseType => String.Empty;

        public override string ExpectedAckType => String.Empty;

        public AckTypes AckType { get; internal set; }

        public GeneralAcknowledgment() : this(new ACK())
        {

        }

  
        public override bool IsResponseForRequest(IHL7Message request)
        {
            throw new NotImplementedException();
        }
    }
}
