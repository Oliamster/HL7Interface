using HL7api.Parser;
using NHapi.Base.Model;
using System;

namespace HL7api.Model
{
    public interface IHL7Message
    {
        /// <summary>
        /// The message class Name
        /// </summary>
        String MessageName { get; }

        string Trigger { get; }

        string Code { get; }

        IMessage Message { get; }
        /// <summary>
        /// The date an time of message (MSH- )
        /// </summary>
        DateTime MessageDateTime { get; }

        /// <summary>
        /// The message control ID (MSH-10
        /// </summary>
        String MessageID { get; }

        /// <summary>
        /// The HL7 version
        /// </summary>
        String MessageVersion { get;  }

        /// <summary>
        /// The name of the expected response to this message
        /// </summary>
        String ExpectedResponseName { get; } 

        /// <summary>
        /// The type of transaction
        /// </summary>
        TransactionType TypeOfTransaction { get; set; }

        /// <summary>
        /// Encode with to the default ER7 encoding and default 
        /// encoding characters
        /// </summary>
        /// <returns></returns>
        String Encode();
        String Encode(HL7Encoding hL7Encoding);

        String GetValue(string path);

        void SetValue(string path, string newValue);
    }
}