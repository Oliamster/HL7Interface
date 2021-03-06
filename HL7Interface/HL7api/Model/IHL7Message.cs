﻿using HL7api.Parser;
using NHapi.Base.Model;
using System;

namespace HL7api.Model
{
    public interface IHL7Message
    {
        /// <summary>
        /// The message class Name
        /// </summary>
        string MessageID { get; }

        /// <summary>
        /// The trigger event
        /// </summary>
        string TriggerEvent { get; }

        /// <summary>
        /// the message code
        /// </summary>
        string MessageCode { get; }

        /// <summary>
        /// The original message
        /// </summary>
        IMessage Message { get; }
        /// <summary>
        /// The date an time of message (MSH- )
        /// </summary>
        DateTime MessageDateTime { get; }

        /// <summary>
        /// The message control ID (MSH-10
        /// </summary>
        string ControlID { get; }

        /// <summary>
        /// The HL7 version
        /// </summary>
        string HL7Version { get;  }

        /// <summary>
        /// The name of the expected response to this message
        /// </summary>
        string ExpectedResponseID { get; }

        /// <summary>
        /// expected response Type
        /// </summary>
        string ExpectedResponseType { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool IsResponseForRequest(IHL7Message request);

        /// <summary>
        /// Whether this is an ack for the supplyed request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        bool IsAckForRequest(IHL7Message request);

        /// <summary>
        /// Whether this message is an acknowledgment
        /// </summary>
        bool IsAcknowledge { get; }

        /// <summary>
        /// the expected ack ID
        /// </summary>
        string ExpectedAckID { get; }

        /// <summary>
        /// expected ack type
        /// </summary>
        string ExpectedAckType { get; }

        /// <summary>
        /// The type of transaction
        /// </summary>
        TransactionType TypeOfTransaction { get; set; }

        /// <summary>
        /// Encode to the default ER7 encoding using the default 
        /// encoding characters
        /// </summary>
        /// <returns></returns>
        string Encode();

        /// <summary>
        /// Encode
        /// </summary>
        /// <param name="hL7Encoding"></param>
        /// <returns></returns>
        string Encode(HL7Encoding hL7Encoding);

        /// <summary>
        /// Get the value from path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string GetValue(string path);

        /// <summary>
        /// Set the value at path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="newValue"></param>
        void SetValue(string path, string newValue);
    }
}