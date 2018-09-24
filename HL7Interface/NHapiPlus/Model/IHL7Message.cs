using NHapi.Base.Model;
using System;

namespace NHapiPlus.Model
{
    public interface IHL7Message
    {
        /// <summary>
        /// The message class Name
        /// </summary>
        String MessageName { get; }

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
        string MessageVersion { get;  }

        /// <summary>
        /// The name of the expected response to this message
        /// </summary>
        String ExpectedResponseName { get; } 

        /// <summary>
        /// The type of transaction
        /// </summary>
        TransactionType TypeOfTransaction { get; set; }
    }
}