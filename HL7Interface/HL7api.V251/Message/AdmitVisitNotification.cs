using NHapi.Model.V251.Message;
using HL7api.Model;
using System;
using NHapi.Model.V251.Datatype;
using NHapi.Base;
using NHapi.Base.Parser;
using System.Diagnostics;

namespace HL7api.V251.Message
{
    public class AdmitVisitNotification : AbstractHL7Message
    {
        protected ADT_A01 aDT_A01;

        public ADT_A01 ADT_A01
        {
            get
            {
                return this.aDT_A01;
            }
        }

        public AdmitVisitNotification() : this(new ADT_A01())
        {

        }

        public AdmitVisitNotification(ADT_A01 aDT_A01)
            : base(aDT_A01)
        {
            this.aDT_A01 = this.m_Message as ADT_A01;
            Debug.Print((new PipeParser()).Encode(aDT_A01));
            this.aDT_A01.MSH.DateTimeOfMessage.Time.SetLongDateWithSecond(DateTime.Now);
            this.aDT_A01.MSH.GetMessageProfileIdentifier(0).EntityIdentifier.Value = GetType().Name;
            this.aDT_A01.MSH.MessageControlID.Value = Guid.NewGuid().ToString();
            this.aDT_A01.MSH.MessageType.MessageCode.Value = "ADT";
            this.aDT_A01.MSH.MessageType.TriggerEvent.Value = "A01";

        }
        public TS DateTimeOfMEssage
        {
            get
            {
                TS ret = null;
                try
                {
                    ret = aDT_A01.MSH.DateTimeOfMessage;
                }
                catch (HL7Exception he)
                { throw new HL7apiException("", he); }
                return ret;
            }
        }


        //public override string ExpectedResponseName =>  

        public override DateTime MessageDateTime
        {
            get
            {
                DateTime ret;
                try
                {
                    ret = DateTime.ParseExact(DateTimeOfMEssage.Time.Value, "yyyyMMddHHmmss", null);
                }
                catch (Exception)
                { throw; }
                return ret;
            }
        }

        public override string ControlID
        {
            get
            {
                string ret = null;
                ret = aDT_A01.MSH.MessageControlID.Value;
                return ret;
            }
        }

        public override string Code => aDT_A01.MSH.MessageType.MessageCode.Value;
        public override string Trigger => aDT_A01.MSH.MessageType.TriggerEvent.Value;

        public override string ExpectedAckID => throw new NotImplementedException();

        public override bool IsAcknowledge => false;
    }
}
