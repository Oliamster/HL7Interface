using NHapi.Base.Parser;
using HL7api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHapi.Base.Model;
using System.Reflection;
using System.Collections.Specialized;
using System.IO;
using NHapi.Base.Util;

namespace HL7api.Parser
{

    /// <summary>
    /// the parser
    /// </summary>
    public class HL7Parser
    {
        private PipeParser pipeParser;
        private XMLParser xmlParser;
        

        public HL7Parser()
        {
            pipeParser = new PipeParser();
            xmlParser = new DefaultXMLParser();
        }
        public IHL7Message Parse(string message, bool validate)
        {
            return DoParse(pipeParser.Parse(message));
        }

        public ParserResult Parse(string message)
        {
            IMessage im = null;
            IHL7Message hl7Message = null;
            ParserResult pr = null;
            try
            {
                im = pipeParser.Parse(message);
                hl7Message = DoParse(im);
                IHL7Message ack = GetAckForMessage(hl7Message);
                pr = new ParserResult(hl7Message, ack, true, hl7Message.ExpectedAckName == null, "Message Accepted");
            }
            catch (Exception ex)
            {
                pr = handleParserException(message, ex);
            }
            return pr;
        }

        private IHL7Message GetAckForMessage(IHL7Message hl7Message)
        {
            string ackName = hl7Message.ExpectedAckName;
            string version = hl7Message.MessageVersion;
            return InstantiateMessage(ackName, version);


        }


        internal IHL7Message InstantiateMessage(string messageID, string version, IMessage message)
        {
            string assemblyName = $"HL7api.V{version.Replace(".", "")}";//.HL7api.V251
            string className = $"{assemblyName}.Message.{messageID}";

            IHL7Message hl7Message = null;
            Type messageType = null;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var a =  AppDomain.CurrentDomain.GetAssemblies().
                       SingleOrDefault(assembly => assembly.GetName().Name == assemblyName);
            
            Assembly.Load(assemblyName);

            String classToTry = $"{className},{assemblyName}";
           
            messageType = Type.GetType(classToTry); //GetType from the specified assembly
            if (messageType == null)
                messageType = Type.GetType(className); //try in the current assembly
            if (messageType == null)
            {
                throw new Exception();
            }
            try
            {
                hl7Message = (IHL7Message)Activator.CreateInstance(messageType, message);
            }
            catch (Exception e)
            {
                throw new HL7apiException("Unable to instantiate the class" + messageID);
            }
            return hl7Message;
        }


        public static Type FindType(string qualifiedTypeName, string assemblyName)
        {
            Type t = Type.GetType(qualifiedTypeName);

            if (t != null)
            {
                return t;
            }
            else
            {
                var assembly = System.Reflection.Assembly.ReflectionOnlyLoadFrom(assemblyName);
                var referencedAssemblies = assembly.GetReferencedAssemblies();

                foreach (var a in referencedAssemblies)
                {
                    if (a.Name == "HL7api.V251")
                        return null;
                }
                return null;
            }
        }


        internal IHL7Message InstantiateMessage(string messageID, string version)
        {
            string assemblyName = $"HL7api.V{version.Replace(".", "")}";//.HL7api.V251
            string className = $"{assemblyName}.Message.{messageID}";

            IHL7Message message = null;
            Type messageType = null;

            String classToTry = $"{className},{assemblyName}";

            messageType = Type.GetType(classToTry); //GetType from the specified assembly
            if (messageType == null)
                messageType = Type.GetType(className); //try in the current assembly
            if (messageType == null)
            {
                throw new Exception();
            }
            try
            {
                message = Activator.CreateInstance(messageType) as IHL7Message;
            }
            catch (Exception e)
            {
                throw new HL7apiException("Unable to instantiate the class" + messageID);
            }
            return message;
        }

        private ParserResult handleParserException(string invalidMessage, Exception ex)
        {
            return new ParserResult(null, null, false, null, ex.Message);
        }

        internal IHL7Message DoParse(IMessage message)
        {
            IHL7Message ret = null;
            if (message == null)
                throw new HL7apiException("Cannot parse an invalid message"); // HL7Exception.APPLICATION_INTERNAL_ERROR);
            Terser t = new Terser(message);


            string profileName = t.Get("/MSH-21(0)-1-1");
            if (string.IsNullOrEmpty(profileName))
            {
                string code = t.Get("MSH-9-1");
                string trigger = t.Get("MSH-9-2");
                profileName = GetMessageIDFromMessageType($"{code}_{trigger}", message.Version);
                if (string.IsNullOrEmpty(profileName))
                    throw new HL7apiException("Invalid Message type");
            }

            ret = (IHL7Message)InstantiateMessage(profileName, message.Version, message);
          
            return ret;
        }




        internal static NameValueCollection GetMapFromRequest(String name, String version)
        {
            NameValueCollection p = null;
            string assemblyName = $"HL7api.V{version.Replace(" ", "")}";
            Assembly assembly = Assembly.Load(assemblyName);
            NameValueCollection map = null;
            try
            {
                map = GetRequestResponseMapping(assembly); ;
            }
            catch (IOException ioe)
            {
                throw new HL7apiException($"Unable to access the application acknowledgment" +
                    $" message type, Cause: {ioe.Message}");
            }
            if (p == null)
                throw new HL7apiException("No map found for version " + version);
            return p;
        }



        internal static string GetMessageIDFromMessageType(string name, string version)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("The name was null or empty");

            if (name.StartsWith("ACK"))
                return null;
            //return typeof(GeneralAcknowledgment).Name;

            string responseType = null;

            NameValueCollection p = GetMapFromRequest(name, version);

            if (!p.AllKeys.Contains(name))
                return null;
                //return typeof(Model.GenericMessage).Name;

            responseType = p.Get(name);
            if (string.IsNullOrEmpty(responseType))
                return null;
            //return typeof(Model.GenericMessage).Name;

            if (responseType.Split(' ').Length == 0)
                return null;
            //return typeof(Model.GenericMessage).Name;

            responseType = responseType.Split(' ')[0];
            return responseType;
        }

        internal static NameValueCollection GetRequestResponseMapping(Assembly assembly)
        {
            NameValueCollection structures = new NameValueCollection();
            using (Stream inResource = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.RequestResponseMap.txt"))
            {
                if (inResource != null)
                {
                    using (StreamReader sr = new StreamReader(inResource))
                    {
                        string line = sr.ReadLine();
                        while (line != null)
                        {
                            if ((line.Length > 0) && ('#' != line[0]))
                            {
                                string[] lineElements = line.Split(',', '\t');
                                structures.Add(lineElements[0], lineElements[1]);
                            }
                            line = sr.ReadLine();
                        }
                    }
                }
            }
            return structures;
        }

        public string Encode (IHL7Message hl7Message, HL7Encoding encoding, bool validate)
        {
            if (HL7Encoding.XML == encoding)
                return xmlParser.Encode(hl7Message.Message);
            return pipeParser.Encode(hl7Message.Message);
        }



        public PipeParser PipeParser => this.pipeParser;

        public string Encode(IHL7Message message)
        {
            return Encode(message, HL7Encoding.ER7, true);
        }
    }
}
