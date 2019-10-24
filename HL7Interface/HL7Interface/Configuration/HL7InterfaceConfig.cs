using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Interface.Configuration
{
    public class HL7InterfaceConfig
    {
        /// <summary>
        /// Gets the name of the protocol you want to use
        /// </summary>
        /// <value>
        /// The name of the server type.
        /// </value>
        public string ProtocolName { get; set; }

        /// <summary>
        /// Gets the type of the protocol
        /// </summary>
        /// <value>
        /// The type of the server.
        /// </value>
        public string ProtocolType { get; set; }


        /// <summary>
        /// Max ack retry number
        /// </summary>
        public int MaxAckRetriesNumber { get; set; }

        /// <summary>
        /// MAx response retry number
        /// </summary>
        public int MaxResponseRetriesNumber { get; set; }

        /// <summary>
        /// Ack time out
        /// </summary>
        public int AckTimeout { get; set; }

        /// <summary>
        /// Response time out
        /// </summary>
        public int ResponseTimeout { get; set; }

        /// <summary>
        /// Either the acknowledgment is required
        /// </summary>
        public bool IsAckRequired { get; set; }

        /// <summary>
        /// Either the ack is blocking
        /// </summary>
        public bool IsAckBlocking { get; set; }


        /// <summary>
        /// Either the acknowledgment is required
        /// </summary>
        public bool IsResponseBlocking { get; set; }


        /// <summary>
        /// Either the response is required
        /// </summary>
        public bool IsResponseRequired { get; set; }

        /// <summary>
        /// Retry on negative acknowledgment
        /// </summary>
        public bool RetryOnNack { get; set; }


        /// <summary>
        /// Gets the send time out.
        /// </summary>
        public int SendTimeOut { get; set; }


        /// <summary>
        /// Gets the start keep alive time, in seconds
        /// </summary>
        public int KeepAliveTime { get; set; }


        /// <summary>
        /// Gets the keep alive interval, in seconds.
        /// </summary>
        public int KeepAliveInterval { get; set; }


        /// <summary>
        /// Gets the child config.
        /// </summary>
        /// <typeparam name="TConfig">The type of the config.</typeparam>
        /// <param name="childConfigName">Name of the child config.</param>
        /// <returns></returns>
        public TConfig GetChildConfig<TConfig>(string childConfigName)
            where TConfig : ConfigurationElement, new()
        {
            return null;
        }

        /// <summary>
        /// Gets the default text encoding.
        /// </summary>
        /// <value>
        /// The text encoding.
        /// </value>
        public string TextEncoding { get; set; }


        /// <summary>
        /// Responds on the same session
        /// </summary>
        public bool RespondOnCurrentSession { get; set; }
    }
}

