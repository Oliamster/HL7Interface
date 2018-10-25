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
        private bool isAccepted = false;
        private bool? isAcknowledge = false;
        private string errorMessage;
        private IHL7Message _parsedMessage;
        private IHL7Message _acknowledge;
        #endregion

        #region Constructors

        internal ParserResult(IHL7Message message, IHL7Message ack, bool isAccepted, bool? isAcknowledge, String errorMessage)
        {
            this._parsedMessage = message;
            this._acknowledge = ack;
            this.isAccepted = isAccepted;
            this.isAcknowledge = isAcknowledge;
            this.errorMessage = errorMessage;
        }
        #endregion

        #region Public Proprieties
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
        public IHL7Message Acknowledge
        {
            get
            {
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

