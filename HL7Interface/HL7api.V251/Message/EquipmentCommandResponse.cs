using NHapi.Model.V251.Message;
using HL7api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHapi.Model.V251.Datatype;
using NHapi.Base.Model.Primitive;
using NHapi.Base;
using System.Globalization;
using NHapi.Base.Parser;
using System.Diagnostics;

namespace HL7api.V251.Message
{
   
    public class EquipmentCommandResponse : AbstractHL7Message
    {
        protected EAR_U08 eAR_U08;

        public EAR_U08 EAR_U08
        {
            get
            {
                return this.eAR_U08;
            }
        }

        public EquipmentCommandResponse() : this(new EAR_U08())
        {

        }

        public EquipmentCommandResponse(EAR_U08 eAR_U08)
            : base(eAR_U08)
        {
            this.eAR_U08 = this.m_Message as EAR_U08;
            Debug.Print((new PipeParser()).Encode(eAR_U08));
            this.eAR_U08.MSH.DateTimeOfMessage.Time.SetLongDateWithSecond(DateTime.Now);
            this.eAR_U08.MSH.GetMessageProfileIdentifier(0).EntityIdentifier.Value = GetType().Name;
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


        //public override string ExpectedResponseName =>  default(string);

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

        public override string MessageID
        {
            get
            {
                string ret = null;
                ret = eAR_U08.MSH.MessageControlID.Value;
                return ret;
            }
        }

        public override string Code => eAR_U08.MSH.MessageType.MessageCode.Value;
        public override string Trigger => eAR_U08.MSH.MessageType.TriggerEvent.Value;



        
    }
}
