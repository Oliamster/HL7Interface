using HL7api.Model;
using NHapi.Base.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7api.Parser
{
    public class ParserResult
    {

        #region Fields
        private IMessage msg;
        private IMessage ack;
        private bool isAccepted = false;
        private bool? isAcknowledge = false;
        private string errorMessage;
        HL7Parser hl7Parser = new HL7Parser();

        private IHL7Message _parsedMessage;
        private IHL7Message _acknowledge;
        #endregion

        #region Constructors

        public ParserResult(IHL7Message message, IHL7Message ack, bool isAccepted, bool? isAcknowledge, String errorMessage)
        {
            this._parsedMessage = message;
            this._acknowledge = ack;
            this.isAccepted = isAccepted;
            this.isAcknowledge = isAcknowledge;
            this.errorMessage = errorMessage;
        }
        #endregion

        #region Public Proprieties

        internal IMessage Message
        {
            get { return msg; }
            set { msg = value; }
        }

        /// <summary>
        /// Either or not the message is accepted. The receiving application should always
        /// send an acknowledgment indipendently on the capability to provide a response.
        /// This propriety is use to avoid processing invalid messages 
        /// </summary>
        public bool IsAccepted
        {
            get { return isAccepted; }
            set { isAccepted = value; }
        }

        /// <summary>
        /// The Acknowledgment should not be acknowledged
        /// </summary>
        public bool ? IsAcknowledge
        {
            get { return isAcknowledge; }
        }

        public string ErrorMessage
        {
            get { return this.errorMessage; }
        }
        #endregion

        /// </summary>
        /// Returns the acknowledgment
        /// <returns> Return null if the message received is an ACK. An ACK 
        /// must not be acknowledged
        /// </returns>
    

        public IHL7Message Acknowledgment
        {
            get
            {
                if (ack == null)
                    return null;
                if (!IsAcknowledge.HasValue)
                    return null;
                if (IsAcknowledge.Value == true)
                    return null;
                return _acknowledge;
            }
        }

        /// <summary>
        /// Return null if the incomming message is not accepted
        /// </summary>
        public IHL7Message ParsedMessage
        {
            get
            {
                if (IsAccepted)
                    return this._parsedMessage;
                return null;
            }
        }
    }
}

