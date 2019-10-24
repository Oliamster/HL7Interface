using System;
using NHapi.Base;
using HL7api.Model;
using NHapi.Base.Parser;
using System.Diagnostics;
using NHapi.Model.V251.Message;
using NHapi.Model.V251.Datatype;

namespace HL7api.V251.Message
{
    public class EquipmentCommandResponse : HL7Message, IHL7Response
    {
        protected EAR_U08 eAR_U08;

        public EAR_U08 EAR_U08
        {
            get
            {
                return this.eAR_U08;
            }
        }

        public override bool IsResponseForRequest(IHL7Message request)
        {
            if (request == null)
                throw new ArgumentNullException("the request and the response should be both non-null");

            if (string.IsNullOrEmpty(request.ExpectedResponseType))
                throw new ArgumentException("the request cannot be a type of response message or acknowledgment");

            if (string.IsNullOrEmpty(this.TriggerEvent) || string.IsNullOrEmpty(this.MessageCode))
                throw new HL7apiException($"The message code and trigger event of the message: " +
                    $" {this.MessageID} are mandatory fields");

            // to avoid situation in which response is actually a request
            if (!string.IsNullOrEmpty(this.ExpectedResponseType))
                return false;

            if (request.ExpectedResponseType != $"{this.MessageCode}_{this.TriggerEvent}")
                return false;

            if ((request.ExpectedResponseID != this.MessageID))
                return false;

            if (request is EquipmentCommandRequest)
            {
                //if (!(this is EquipmentCommandResponse))
                //    return false;

                //if (request.GetValue("COMMAND(0)/ECD-2-1")
                //    != this.GetValue("COMMAND_RESPONSE(0)/ECD-2-1"))
                //    return false;

                //if (request.GetValue("COMMAND(0)/SPECIMEN_CONTAINER/SAC-3-1")
                //    != this.GetValue("COMMAND_RESPONSE(0)/SPECIMEN_CONTAINER/SAC-3-1"))
                //    return false;
            }
            return true;
        }

        public EquipmentCommandResponse() : this(new EAR_U08())
        {

        }

        public EquipmentCommandResponse(EquipmentCommandRequest request) :this()
        {

        }


        public EquipmentCommandResponse(EAR_U08 eAR_U08)
            : base(eAR_U08)
        {
            this.eAR_U08 = this.m_Message as EAR_U08;
            Debug.Print((new PipeParser()).Encode(eAR_U08));
            this.eAR_U08.MSH.DateTimeOfMessage.Time.SetLongDateWithSecond(DateTime.Now);
            this.eAR_U08.MSH.GetMessageProfileIdentifier(0).EntityIdentifier.Value = GetType().Name;

            string msh10 = this.eAR_U08.MSH.MessageControlID.Value;
            if (string.IsNullOrEmpty(msh10))
                this.eAR_U08.MSH.MessageControlID.Value = Guid.NewGuid().ToString();

            this.eAR_U08.MSH.MessageType.MessageCode.Value = "EAR";
            this.eAR_U08.MSH.MessageType.TriggerEvent.Value = "U08";

        }
        public TS DateTimeOfMEssage
        {
            get
            {
                TS ret = null;
                try
                {
                    ret = eAR_U08.MSH.DateTimeOfMessage;
                }
                catch (HL7Exception he)
                { throw new HL7apiException("", he); }
                return ret;
            }
        }


        public override string ExpectedResponseID =>  string.Empty;

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
                ret = eAR_U08.MSH.MessageControlID.Value;
                return ret;
            }
        }

        public override string MessageCode => eAR_U08.MSH.MessageType.MessageCode.Value;
        public override string TriggerEvent => eAR_U08.MSH.MessageType.TriggerEvent.Value;

        public override string ExpectedAckID => typeof(GeneralAcknowledgment).Name;

        public override bool IsAcknowledge => false;

        public override string ExpectedResponseType => String.Empty;

        public override string ExpectedAckType => "ACK_U08";

        public string RequestID { get; set; }
    }
}
