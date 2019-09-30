using NHapi.Model.V251.Message;
using HL7api.Model;
using System;
using NHapi.Model.V251.Datatype;
using NHapi.Base;
using NHapi.Base.Parser;
using System.Diagnostics;

namespace HL7api.V251.Message
{
    public class EquipmentCommandRequest : AbstractHL7Message
    {
        #region Fields
        protected EAC_U07 eAC_U07; 
        #endregion

        #region Constructors
        public EquipmentCommandRequest() : this(new EAC_U07())
        {

        }

        public EquipmentCommandRequest(EAC_U07 eAC_U07)
            : base(eAC_U07)
        {
            this.eAC_U07 = this.m_Message as EAC_U07;
            Debug.Print((new PipeParser()).Encode(eAC_U07));
            this.eAC_U07.MSH.DateTimeOfMessage.Time.SetLongDateWithSecond(DateTime.Now);
            this.eAC_U07.MSH.GetMessageProfileIdentifier(0).EntityIdentifier.Value = GetType().Name;

            string msh10 = this.eAC_U07.MSH.MessageControlID.Value;

            if (string.IsNullOrEmpty(msh10))
                this.eAC_U07.MSH.MessageControlID.Value = Guid.NewGuid().ToString();

            this.eAC_U07.MSH.MessageType.MessageCode.Value = "EAC";
            this.eAC_U07.MSH.MessageType.TriggerEvent.Value = "U07";
        }
        #endregion

        #region Properties
        public EAC_U07 EAC_U07
        {
            get
            {
                return this.eAC_U07;
            }
        }
      
        public TS DateTimeOfMessage
        {
            get
            {
                TS ret = null;
                try
                {
                    ret = eAC_U07.MSH.DateTimeOfMessage;
                }
                catch (HL7Exception he)
                { throw new HL7apiException("", he); }
                return ret;
            }
        }

        public override string ExpectedResponseID => typeof(EquipmentCommandResponse).Name;

        public override DateTime MessageDateTime
        {
            get
            {
                DateTime ret;
                try
                {
                    ret = DateTime.ParseExact(DateTimeOfMessage.Time.Value, "yyyyMMddHHmmss", null);
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
                ret = eAC_U07.MSH.MessageControlID.Value;
                return ret;
            }
        }

        public override string Code => eAC_U07.MSH.MessageType.MessageCode.Value;
        public override string Trigger => eAC_U07.MSH.MessageType.TriggerEvent.Value;

        public override string ExpectedAckID => typeof(GeneralAcknowledgment).Name;

        public override bool IsAcknowledge => false;

        public override string ExpectedResponseType => "EAR_U08";

        public override string ExpectedAckType => "ACK_U07";

        public override bool IsResponseForRequest(IHL7Message request)
        {
            return false;
        }
        #endregion
    }
}
