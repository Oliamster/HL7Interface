using System;
using System.Runtime.Serialization;

namespace NHapiPlus.V251.Message
{
    [Serializable]
    internal class NHapiPlusException : Exception
    {
        public NHapiPlusException()
        {
        }

        public NHapiPlusException(string message) : base(message)
        {
        }

        public NHapiPlusException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NHapiPlusException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}