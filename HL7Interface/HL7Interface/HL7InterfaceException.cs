using System;
using System.Runtime.Serialization;

namespace HL7Interface
{
    [Serializable]
    public class HL7InterfaceException : Exception
    {
        public HL7InterfaceException()
        {
        }

        public HL7InterfaceException(string message) : base(message)
        {
        }

        public HL7InterfaceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HL7InterfaceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}