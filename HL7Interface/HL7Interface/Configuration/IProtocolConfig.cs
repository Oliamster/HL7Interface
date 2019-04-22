using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Interface.Configuration
{
    public interface IProtocolConfig
    {
        /// <summary>
        /// Gets the name of the protocol you use
        /// </summary>
        /// <value>
        /// The name of the protocol.
        /// </value>
        string ProtocolName { get; }

        /// <summary>
        /// Gets the type of the protocol
        /// </summary>
        /// <value>
        /// The type of the protocol.
        /// </value>
        string ProtocolType { get; }

        /// <summary>
        /// Max ack retry number
        /// </summary>
        int MaxAckRetriesNumber { get; }

        /// <summary>
        /// Max response retry number
        /// </summary>
        int MaxResponseRetriesNumber { get; }

        /// <summary>
        /// Ack time out
        /// </summary>
        int AckTimeout { get; }

        /// <summary>
        /// Response time out
        /// </summary>
        int ResponseTimeout { get; }

        /// <summary>
        /// whether the response is send back to the sender using the same connection.
        /// i.e, the same session the default value is true.
        /// </summary>
        bool RespondOnCurrentSession { get; }

        /// <summary>
        /// Either the acknowledgment is required
        /// </summary>
        bool IsAckRequired { get; }

        /// <summary>
        /// Either the ack is blocking
        /// </summary>
        bool IsAckBlocking { get; }

        /// <summary>
        /// Either the acknowledgment is required
        /// </summary>
        bool IsResponseBlocking { get; }

        /// <summary>
        /// Either the response is required
        /// </summary>
        bool IsResponseRequired { get; }

        /// <summary>
        /// Retry on negative acknowledgment
        /// </summary>
        bool RetryOnNack { get; }

        /// <summary>
        /// Gets the send time out.
        /// </summary>
        int SendTimeOut { get; }

        /// <summary>
        /// Gets the start keep alive time, in seconds
        /// </summary>
        int KeepAliveTime { get; }

        /// <summary>
        /// Gets the keep alive interval, in seconds.
        /// </summary>
        int KeepAliveInterval { get; }

        /// <summary>
        /// Gets the child config.
        /// </summary>
        /// <typeparam name="TConfig">The type of the config.</typeparam>
        /// <param name="childConfigName">Name of the child config.</param>
        /// <returns></returns>
        TConfig GetChildConfig<TConfig>(string childConfigName)
            where TConfig : ConfigurationElement, new();
  
        /// <summary>
        /// Gets the default text encoding.
        /// </summary>
        /// <value>
        /// The text encoding.
        /// </value>
        string TextEncoding { get; }
    }
}

