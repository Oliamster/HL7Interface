using System;
using System.Runtime.Serialization;

namespace HL7api
{
    [Serializable]
    public class HL7apiException : Exception
    {
        public HL7apiException()
        {
        }

        public HL7apiException(string message) : base(message)
        {
        }

        public HL7apiException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HL7apiException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}