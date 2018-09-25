using NHapi.Model.V251.Message;
using NHapiPlus.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHapi.Model.V251.Datatype;
using NHapi.Base.Model.Primitive;
using NHapi.Base;

namespace NHapiPlus.V251.Message
{
    public class AdmitVisitNotification  : AbstractHL7Message
    {
        protected ADT_A01 aDT_A01;

        public ADT_A01 ADT_A01
        {
            get
            {
                return this.m_Message as ADT_A01;
            }
        }

        public AdmitVisitNotification() : this(new ADT_A01())
        {
            
        }

        public AdmitVisitNotification(ADT_A01 aDT_A01)
            : base(aDT_A01)
        {
            this.aDT_A01 = this.m_Message as ADT_A01;
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
                { throw new NHapiPlusException("", he); }
                return ret;
            } 
        }

        public override DateTime MessageDateTime
        {
            get
            {
                DateTime ret;
                try
                {
                    ret = DateTime.Parse(DateTimeOfMEssage.Time.Value);
                }
                catch (Exception)
                {   throw;   }
                return ret;
            }
        }
    }
}
