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
using System.Diagnostics;

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
            Validator val = new Validator();
            return val.Validate(message);
        }

        public IHL7Message GetAckForMessage(IHL7Message hl7Message)
        {
            string ackName = hl7Message.ExpectedAckID;
            string version = hl7Message.HL7Version;
            return InstantiateMessage(ackName, version);
        }


        internal IHL7Message InstantiateMessage(string messageID, string version, IMessage message)
        {
            string assemblyName = $"HL7api.V{version.Replace(".", "")}"; //.HL7api.V251

            string className = $"{assemblyName}.Message.{messageID}";

            IHL7Message hl7Message = null;

            Type messageType = null;
            String classToTry = $"{className},{assemblyName}";
           
            messageType = Type.GetType(classToTry); //GetType from the specified assembly

            if (messageType == null)
                messageType = Type.GetType(className); //try in the current assembly

            if (messageType == null)
            {
                throw new HL7apiException();
            }
            try
            {

                ConstructorInfo[] cis = messageType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

                 hl7Message = (IHL7Message)Activator.CreateInstance(
                     messageType, BindingFlags.NonPublic | BindingFlags.Instance, 
                     null,
                     new object[] { message }, 
                     null);
            }
            catch (Exception e)
            {
                


                throw new HL7apiException("Unable to instantiate the class " + messageID);
            }
            return hl7Message;
        }



        public static bool IsAckForRequest(IHL7Message request, IHL7Message ack)
        {
            if (request == null)
                throw new ArgumentNullException("Please provide a non-null value for the request");

            if (ack == null)
                return false;

            if (string.IsNullOrEmpty(request.ExpectedAckID))
                throw new HL7apiException("The request should not be an acknowledgment");

            if (!string.IsNullOrEmpty(ack.ExpectedAckID))
                throw new HL7apiException("The ack should be an acknowledgment");

            string msa2 = ack.GetValue("/MSA-2");
            string msh10 = request.ControlID;

            if (string.IsNullOrEmpty(msa2) || string.IsNullOrEmpty(msh10))
                throw new HL7apiException("The MSH-10 and MSA-2 are mandatory in the messages:"
                    + request.MessageID + "and" + ack.MessageID);

            if (String.Compare(msa2, msh10) != 0)
                return false;
            
            //if the trigger event is provided then check aigain
            if (!string.IsNullOrEmpty(ack.Trigger))
            {
                if (!request.ExpectedAckType.Equals($"{ack.Code}_{ack.Trigger}"))
                    return false;
            }
            else
            {
                if (!ack.Code.Equals("ACK"))
                    return false;
            }
            return true;
        }



        internal static string GetAckTypeFromRequest(string name, string version)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            if (name.StartsWith("ACK"))
                return null;

            string responseType = null;

            if (!PipeParser.ValidVersion(version))
                throw new HL7apiException("Invalid HL7 Version");

            NameValueCollection p = GetMapFromRequest(name, version);
            if (!p.AllKeys.Contains(name))
                return "ACK";
            responseType = p.Get(name);
            if (responseType == null)
                return null;
            else
            {
                if (responseType.Split(' ').Length > 1)
                {
                    responseType = responseType.Split(' ')[1];
                }
                else
                    return null;
            }

            return responseType;
        }

        internal static string correctName(string ret)
        {
            throw new NotImplementedException();
        }

        internal IMessage InstantiateMessage(String theName, String theVersion, bool isExplicit)
        {
            IMessage result = null;
            IModelClassFactory myFactory = new DefaultModelClassFactory();
            if (theName.StartsWith("ACK"))
                theName = "ACK";
            Type messageClass = myFactory.GetMessageClass(theName, theVersion, isExplicit);
            if (messageClass == null)
            {
                throw new HL7apiException("Can't find message class in current package list: " + theName);
            }
            ConstructorInfo constructor = messageClass.GetConstructor(
                new Type[] { typeof(IModelClassFactory) });
            result = (IMessage)constructor.Invoke(new Object[] { myFactory });
            return result;
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
                throw new HL7apiException("Unable to instantiate the class" + messageID, e);
            }
            return message;
        }

        private ParserResult handleParserException(string invalidMessage, Exception ex)
        {
            return new ParserResult(null, null, false, null, ex.Message);
        }

        internal IHL7Message DoParse(IMessage message)
        {
            if (message == null)
                throw new HL7apiException("Cannot parse an invalid message");

            IHL7Message ret = null;

            Terser t = new Terser(message);
          

            string messageID = t.Get("/MSH-21(0)-1-1");

            if (string.IsNullOrEmpty(messageID))
            {
                messageID = GetMessageIDFromMessageType(message.GetMessageType(), message.Version);
                if (string.IsNullOrEmpty(messageID))
                    throw new HL7apiException("Invalid Message type");
            }

            ret = (IHL7Message)InstantiateMessage(messageID, message.Version, message);
          
            return ret;
        }




        internal static NameValueCollection GetMapFromRequest(String name, String version)
        {
            version = version.Replace(".", "");
            string assemblyName = $"HL7api.V{version}";
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
            if (map == null)
                throw new HL7apiException("No map found for version " + version);
            return map;
        }



        internal static string GetMessageIDFromMessageType(string name, string version)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("The name was null or empty");

            if (name.StartsWith("ACK"))
                return "GeneralAcknowledgment";
            
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

    public class MapClass
    {
        string MessageType;

        string AckType;

        string AckID;

        string expectedAck;

        string MessageID;
    }
}
