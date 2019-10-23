﻿using HL7api.Model;
using SuperSocket.SocketBase.Protocol;
using System.Threading;

namespace HL7Interface
{
    public class HL7Request : IRequestInfo
    {
        public string Key
        {
            get; set;
        }

        public IHL7Message Request
        {
            get; set;
        }

        public IHL7Message Acknowledgment
        {
            get; set;
        }

        public IHL7Message Response
        {
            get; set;
        }

        protected AutoResetEvent ResponseReceivedEvent
        {
            get; set;
        }
    }
}