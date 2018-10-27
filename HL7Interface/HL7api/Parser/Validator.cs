using System;
using System.Text;
using NHapi.Base;
using NHapi.Base.Model;
using NHapi.Base.Parser;
using NHapi.Base.Util;
using System.Configuration;
using HL7api.Model;
using NHapiTools.Base.Util;

namespace HL7api.Parser
{

    internal class Validator
    {
        #region Fields
        private IHL7Message parsedMessage;
        private IHL7Message hl7Ack;
        private PipeParser contextParser;
        private HL7Parser hl7Parser;
        private StringBuilder _errorMessage = new StringBuilder("ERROR MESSAGE: ");
        private EncodingCharacters enchars = new EncodingCharacters('|', null);
        #endregion

        #region Constructors
        public Validator()
        {
            this.hl7Parser = new HL7Parser();
            contextParser = new PipeParser();
        }

        public Validator(PipeParser contextParser, HL7Parser hl7Parser)
        {
            this.contextParser = contextParser;
            this.hl7Parser = hl7Parser;
        }
        #endregion

        #region Methods
        internal ParserResult Validate(String messageString)
        {
            IMessage messageObject = null;
            try
            {
                messageObject = contextParser.Parse(messageString);
            }
            catch (System.Exception se)
            {
                return SystemExceptionHandler(se, messageString);
            }

            parsedMessage = hl7Parser.DoParse(messageObject);

            if (parsedMessage.IsAcknowledge)
                return new ParserResult(parsedMessage, null, true, true, "");

            hl7Ack = hl7Parser.GetAckForMessage(parsedMessage);

            SetACK(AckTypes.AA, messageString, parsedMessage.Message);

            return new ParserResult(parsedMessage, hl7Ack, true, false, "");
        }
        #endregion


        private ParserResult SystemExceptionHandler(Exception se, string messageString)
        {
            if (string.IsNullOrEmpty(messageString))
                return new ParserResult(null, null, false, null, se.Message);

            //Do not handle configuration Exceptions
            if (se is ConfigurationException)
                throw (ConfigurationException)se;

            if (typeof(System.Configuration.ConfigurationException).IsAssignableFrom(se.GetType()))
                throw (ConfigurationException)se;

            _errorMessage.AppendLine(se.Message);

            if (se.InnerException != null)
                _errorMessage.AppendLine(se.InnerException.Message);

            string messageType = null;

            string version = null;

            ISegment criticalSegment = null;

            try
            {
                criticalSegment = TryToRecoverCriticalDataFromMessage(messageString);
                if (criticalSegment == null)
                {
                    version = TryToRecoverTheVersion(messageString);
                    messageType = TryTorecoverTheMessageType(messageString);
                }
                else
                {
                    version = Terser.Get(criticalSegment, 12, 0, 1, 1);
                    messageType = $"{Terser.Get(criticalSegment, 9, 0, 1, 1)}" +
                        $"_{Terser.Get(criticalSegment, 9, 0, 2, 1)}";
                }

                if (messageType == null)
                    return new ParserResult(null, null, false, null, _errorMessage.ToString());

                //the messageType is not null!
                if (version == null)
                    version = "2.5.1";


                string responseType = HL7Parser.GetMessageIDFromMessageType(messageType, version);
                if (responseType == null)
                    //the incoming message is an ack, the acknowledgment should not be aknoledged
                    return new ParserResult(null, null, false, true, _errorMessage.ToString());


                hl7Ack = hl7Parser.InstantiateMessage(responseType, version);
               
                ISegment err = (ISegment)hl7Ack.Message.GetStructure("ERR", 0);
                if ((se.InnerException != null) && typeof(HL7Exception).IsAssignableFrom(se.InnerException.GetType()))
                {
                    HL7Exception he = (HL7Exception)se.InnerException;
                    he.populate(err);
                }

                //if (typeof(HL7Exception).IsAssignableFrom(se.GetType()))
                //{
                //    HL7Exception he = (HL7Exception)se;
                //    he.populate(err);
                //    Debug.Print((new PipeParser()).Encode(ack));
                //}

                SetACK(AckTypes.AR, messageString);
            }
            catch (Exception e)
            {
                _errorMessage.AppendLine(e.Message);
                return new ParserResult(parsedMessage, hl7Ack, false, null, _errorMessage.ToString());
            }
            return new ParserResult(parsedMessage, hl7Ack, false, parsedMessage.IsAcknowledge, _errorMessage.ToString());
        }


        private string TryToRecoverTheVersion(string messageString)
        {
            String version = null;

            String wholeField12;
            try
            {
                String[] fields = PipeParser.Split(messageString.Substring(0, (Math.Max(messageString.IndexOf("\r"), messageString.Length)) - (0)),
                    Convert.ToString(enchars.FieldSeparator));
                wholeField12 = fields[11];
                //message structure is component 3 but we'll accept a composite of 1 and 2 if there is no component 3 ...
                //      if component 1 is ACK, then the structure is ACK regardless of component 2
                String[] comps = PipeParser.Split(wholeField12, Convert.ToString(enchars.ComponentSeparator));

                if (comps.Length > 0)
                {
                    //messageStructure = comps[2];
                    version = comps[0];
                }
                else
                {
                    StringBuilder buf = new StringBuilder("Can't determine message version from MSH-12: ");
                    throw new HL7Exception(buf.ToString(), HL7Exception.UNSUPPORTED_VERSION_ID);
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new HL7Exception("Can't find message structure (MSH-9-3): " + e.Message, HL7Exception.UNSUPPORTED_MESSAGE_TYPE);

            }
            return ParserBase.ValidVersion(version) ? version : null;
        }

        private string TryTorecoverTheMessageType(string messageString)
        {
            EncodingCharacters ec = new EncodingCharacters('|', null);
            String messageStructure = null;
            bool explicityDefined = true;
            String wholeFieldNine;
            try
            {
                String[] fields = PipeParser.Split(messageString.Substring(0, (Math.Max(messageString.IndexOf("\r"), messageString.Length)) - (0)),
                    Convert.ToString(ec.FieldSeparator));
                wholeFieldNine = fields[8];
                //message structure is component 3 but we'll accept a composite of 1 and 2 if there is no component 3 ...
                //      if component 1 is ACK, then the structure is ACK regardless of component 2
                String[] comps = PipeParser.Split(wholeFieldNine, Convert.ToString(ec.ComponentSeparator));

                if (comps.Length >= 3)
                {
                    //messageStructure = comps[2];
                    messageStructure = comps[0] + "_" + comps[1];
                }
                else if (comps.Length > 0 && comps[0] != null && comps[0].Equals("ACK"))
                {
                    messageStructure = "ACK";
                }
                else if (comps.Length == 2)
                {
                    explicityDefined = false;
                    messageStructure = comps[0] + "_" + comps[1];
                }
                /*else if (comps.length == 1 && comps[0] != null && comps[0].equals("ACK")) {
            messageStructure = "ACK"; //it's common for people to only populate component 1 in an ACK msg
            }*/
                else
                {
                    StringBuilder buf = new StringBuilder("Can't determine message structure from MSH-9: ");
                    buf.Append(wholeFieldNine);
                    if (comps.Length < 3)
                    {
                        buf.Append(" HINT: there are only ");
                        buf.Append(comps.Length);
                        buf.Append(" of 3 components present");
                    }
                    throw new HL7Exception(buf.ToString(), HL7Exception.UNSUPPORTED_MESSAGE_TYPE);
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new HL7Exception("Can't find message structure (MSH-9-3): " + e.Message, HL7Exception.UNSUPPORTED_MESSAGE_TYPE);

            }

            return messageStructure;
        }

        internal void SetACK(AckTypes ackType, String messageString, IMessage message)
        {
            if (message == null)
            {
                SetACK(ackType, messageString);
                return;
            }
            Terser inboundTerser = new Terser(message);
            ISegment inboundHeader = inboundTerser.getSegment("MSH");

            string version = null;
            version = message.Version;
            if (string.IsNullOrEmpty(version))
            {
                try
                {
                    version = Terser.Get(inboundHeader, 12, 0, 1, 1);
                }
                catch (NHapi.Base.HL7Exception he)
                {
                    version = NHapi.Model.V251.Constants.VERSION;
                }
            }

            FillMSHSegment((ISegment)message.GetStructure("MSH"), (ISegment)hl7Ack.Message.GetStructure("MSH"));

          
            string messageTypeName = message.GetMessageType();
            string acktype = HL7Parser.GetAckTypeFromRequest(messageTypeName, version);
            string AckID = HL7Parser.GetMessageIDFromMessageType(acktype, version);
            hl7Ack.SetValue("/MSH(0)-21-1", AckID); //MEssage Code

            if (acktype.Split('_').Length > 1)
            {
                hl7Ack.SetValue("/MSH-9-1-1", acktype.Split('_')[0]); //MEssage Code
                hl7Ack.SetValue("/MSH-9-2-1", acktype.Split('_')[1]); //MEssage Code
            }
            //ackTerser.Set("/MSH-12", NHapi.Model.V251.Constants.VERSION);
            hl7Ack.SetValue("/MSA-1", Enum.GetName(typeof(AckTypes), ackType));
            hl7Ack.SetValue("/MSA-2", Terser.Get(inboundHeader, 10, 0, 1, 1));
        }




        private void SetACK(AckTypes ackType, String messageString)
        {
            Terser terser = new Terser(hl7Ack.Message);
            ISegment err = terser.getSegment("/ERR(0)");
            ISegment criticalSegment = TryToRecoverCriticalDataFromMessage(messageString);


            try
            {
                FillMSHSegment((ISegment)hl7Ack.Message.GetStructure("MSH"), criticalSegment);

                if (criticalSegment == null)
                {
                    terser.Set("/MSA-1", Enum.GetName(typeof(AckTypes), ackType));
                    terser.Set("/MSA-2", "");
                }
                else
                {
                    terser.Set("/MSA-1", Enum.GetName(typeof(AckTypes), ackType));
                    terser.Set("/MSA-2", Terser.Get(criticalSegment, 10, 0, 1, 1));
                }
            }
            catch (HL7Exception he)
            {

                _errorMessage.Append(he.Message);
                return; //do not throw exceptions;
            }
             //terser.Set("ERR(0)-7", _errorMessage);
            terser.Set("/ERR(0)-7-1-1", _errorMessage.ToString());
        }


        private ISegment TryToRecoverCriticalDataFromMessage(string message)
        {
            ISegment msh = null;

            if (!message.StartsWith("MSH|"))
                return null;
            //try to get MSH segment
            int locStartMSH = message.IndexOf("MSH");
            if (locStartMSH < 0)
                return null;
            int locEndMSH = message.IndexOf('\r', locStartMSH + 1);
            if (locEndMSH < 0)
                locEndMSH = message.Length;
            String mshString = message.Substring(locStartMSH, (locEndMSH) - (locStartMSH));

            //find out what the field separator is
            char fieldSep = mshString[3];

            //get field array
            String[] fields = PipeParser.Split(mshString, Convert.ToString(fieldSep));

            try
            {
                //parse required fields
                String encChars = fields[1];
                char compSep = encChars[0];
                String messControlID = fields[9];
                String[] procIDComps = PipeParser.Split(fields[10], Convert.ToString(compSep));

                //fill MSH segment
                String version = NHapi.Model.V251.Constants.VERSION; //default
                try
                {
                    version = contextParser.GetVersion(message);
                }
                catch (Exception)
                {
                    /* use the default */
                }
                //IModelClassFactory factory = parser.Factory;
                //msh = new MSH(new NHapiBugsFixing.V251(factory), factory);

                //msh = new GenericSegment(new NHapiBugsFixing.GenericMessageVersion(version), "MSH");

                Terser.Set(msh, 1, 0, 1, 1, Convert.ToString(fieldSep));
                Terser.Set(msh, 2, 0, 1, 1, encChars);
                Terser.Set(msh, 10, 0, 1, 1, messControlID);
                Terser.Set(msh, 12, 0, 1, 1, version);

                string messageType = TryTorecoverTheMessageType(message);

                if (!string.IsNullOrEmpty(messageType))
                {
                    Terser.Set(msh, 9, 0, 1, 1, messageType.Split('_')[0]);
                    if (messageType.Split('_').Length > 0)
                        Terser.Set(msh, 9, 0, 2, 1, messageType.Split('_')[1]);
                }

                //Terser.Set(msh, 11, 0, 1, 1, procIDComps[0]);
            }
            catch (HL7Exception he)
            {
                _errorMessage.AppendLine(he.Message);
                return null;
            }
            catch (System.Exception se)
            {
                _errorMessage.AppendLine(se.Message);
                return null;
            }
            return msh;
        }
        

        private void FillMSHSegment(ISegment mshIn, ISegment mshOut)
        {
            if (mshOut == null)
                throw new System.Exception();

            Guid g = Guid.NewGuid();
            Terser.Set(mshIn, 7, 0, 1, 1, DateTime.Now.ToString("yyyyMMddHHmmss"));
            Terser.Set(mshIn, 7, 0, 1, 1, DateTime.Now.ToString("yyyyMMddHHmmss"));

            if (mshIn == null)
            {
                Terser.Set(mshIn, 3, 0, 1, 1, "");
                Terser.Set(mshIn, 4, 0, 1, 1, "");
                Terser.Set(mshIn, 5, 0, 1, 1, "");
                Terser.Set(mshIn, 6, 0, 1, 1, "");
                Terser.Set(mshIn, 9, 0, 1, 1, "ACK");
            }
            else
            {
                Terser.Set(mshIn, 3, 0, 1, 1, Terser.Get(mshIn, 5, 0, 1, 1));
                Terser.Set(mshIn, 4, 0, 1, 1, Terser.Get(mshIn, 6, 0, 1, 1));
                Terser.Set(mshIn, 5, 0, 1, 1, Terser.Get(mshIn, 3, 0, 1, 1));
                Terser.Set(mshIn, 6, 0, 1, 1, Terser.Get(mshIn, 4, 0, 1, 1));
                Terser.Set(mshIn, 9, 0, 1, 1, Terser.Get(mshIn, 9, 0, 1, 1));
                Terser.Set(mshIn, 9, 0, 2, 1, Terser.Get(mshIn, 9, 0, 2, 1)); 
            }
        }
    }
}


