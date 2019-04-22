using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Security.Authentication;
using System.Text;
using System.Xml;
using System.Linq;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;

namespace  HL7Interface.Configuration
{
    /// <summary>
    /// protocol configuration
    /// </summary>
    public  class ProtocolConfig : ConfigurationElementBase, IProtocolConfig
    {
        /// <summary>
        /// Gets the name of the protocol 
        /// </summary>
        /// <value>
        /// The name of the server type.
        /// </value>
        [ConfigurationProperty("protocolName", IsRequired = false)]
        public string ProtocolName
        {
            get { return this["protocolName"] as string; }
        }

        /// <summary>
        /// Gets the type definition of the appserver.
        /// </summary>
        /// <value>
        /// The type of the server.
        /// </value>
        [ConfigurationProperty("ProtocolType", IsRequired = false)]
        public string ProtocolType
        {
            get { return this["ProtocolType"] as string; }
        }

        /// <summary>
        /// Gets the Receive filter factory.
        /// </summary>
        [ConfigurationProperty("MaxAckRetriesNumber", IsRequired = false)]
        public int MaxAckRetriesNumber
        {
            get { return (int)this["MaxAckRetriesNumber"]; }
        }

        /// <summary>
        /// Gets the ip.
        /// </summary>
        [ConfigurationProperty("MaxResponseRetriesNumber", IsRequired = false)]
        public int MaxResponseRetriesNumber
        {
            get { return (int)this["MaxResponseRetriesNumber"]; }
        }

        /// <summary>
        /// Gets the port.
        /// </summary>
        [ConfigurationProperty("AckTimeout", IsRequired = false)]
        public int AckTimeout
        {
            get { return (int)this["AckTimeout"]; }
        }

        /// <summary>
        /// Gets the mode.
        /// </summary>
        [ConfigurationProperty("ResponseTimeout", IsRequired = false)]
        public int ResponseTimeout
        {
            get { return (int)this["ResponseTimeout"]; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        ///   <c>true</c> if [sync send]; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty("respondOnCurrentSession", IsRequired = false, DefaultValue = true)]
        public bool RespondOnCurrentSession
        {
            get { return (bool)this["respondOnCurrentSession"]; }
        }


        /// <summary>
        /// Gets the send time out.
        /// </summary>
        [ConfigurationProperty("sendTimeOut", IsRequired = false, DefaultValue = ServerConfig.DefaultSendTimeout)]
        public int SendTimeOut
        {
            get { return (int)this["sendTimeOut"]; }
        }


        /// <summary>
        /// Gets the size of the receive buffer.
        /// </summary>
        /// <value>
        /// The size of the receive buffer.
        /// </value>
        [ConfigurationProperty("receiveBufferSize", IsRequired = false, DefaultValue = ServerConfig.DefaultReceiveBufferSize)]
        public int ReceiveBufferSize
        {
            get { return (int)this["receiveBufferSize"]; }
        }


        /// <summary>
        /// Gets a value indicating whether sending is in synchronous mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [sync send]; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty("IsAckRequired", IsRequired = false, DefaultValue = false)]
        public bool IsAckRequired
        {
            get { return (bool)this["IsAckRequired"]; }
        }



        /// <summary>
        /// Gets a value indicating whether sending is in synchronous mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [sync send]; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty("IsResponseBlocking", IsRequired = false, DefaultValue = false)]
        public bool IsResponseBlocking
        {
            get { return (bool)this["IsResponseBlocking"]; }
        }


        /// <summary>
        /// Either the response is required or not
        /// </summary>
        /// <value>
        ///   <c>true</c> if [sync send]; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty("IsResponseRequired", IsRequired = false, DefaultValue = false)]
        public bool IsResponseRequired
        {
            get { return (bool)this["IsResponseRequired"]; }
        }


        /// <summary>
        /// Gets a value indicating whether sending is in synchronous mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [sync send]; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty("RetryOnNack", IsRequired = false, DefaultValue = false)]
        public bool RetryOnNack
        {
            get { return (bool)this["RetryOnNack"]; }
        }

        /// <summary>
        /// Gets a value indicating whether log command in log file.
        /// </summary>
        /// <value><c>true</c> if log command; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("IsAckBlocking", IsRequired = false, DefaultValue = false)]
        public bool IsAckBlocking
        {
            get { return (bool)this["IsAckBlocking"]; }
        }

     

        /// <summary>
        /// Gets the start keep alive time, in seconds
        /// </summary>
        [ConfigurationProperty("keepAliveTime", IsRequired = false, DefaultValue = ServerConfig.DefaultKeepAliveTime)]
        public int KeepAliveTime
        {
            get
            {
                return (int)this["keepAliveTime"];
            }
        }

        /// <summary>
        /// Gets the keep alive interval, in seconds.
        /// </summary>
        [ConfigurationProperty("keepAliveInterval", IsRequired = false, DefaultValue = ServerConfig.DefaultKeepAliveInterval)]
        public int KeepAliveInterval
        {
            get
            {
                return (int)this["keepAliveInterval"];
            }
        }

   
  

        /// <summary>
        /// Gets the default text encoding.
        /// </summary>
        /// <value>
        /// The text encoding.
        /// </value>
        [ConfigurationProperty("textEncoding", IsRequired = false, DefaultValue = "")]
        public string TextEncoding
        {
            get
            {
                return (string)this["textEncoding"];
            }
        }
 
        /// <summary>
        /// Gets the child config.
        /// </summary>
        /// <typeparam name="TConfig">The type of the config.</typeparam>
        /// <param name="childConfigName">Name of the child config.</param>
        /// <returns></returns>
        public TConfig GetChildConfig<TConfig>(string childConfigName)
            where TConfig : ConfigurationElement, new()
        {
            return this.OptionElements.GetChildConfig<TConfig>(childConfigName);
        }

        /// <summary>
        /// Gets a value indicating whether an unknown attribute is encountered during deserialization.
        /// To keep compatible with old configuration
        /// </summary>
        /// <param name="name">The name of the unrecognized attribute.</param>
        /// <param name="value">The value of the unrecognized attribute.</param>
        /// <returns>
        /// true when an unknown attribute is encountered while deserializing; otherwise, false.
        /// </returns>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            //To keep compatible with old configuration
            if (!"serviceName".Equals(name, StringComparison.OrdinalIgnoreCase))
                return base.OnDeserializeUnrecognizedAttribute(name, value);

            this["serverTypeName"] = value;
            return true;
        }
    }
}
