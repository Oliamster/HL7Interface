
using HL7api.Parser;
using Hl7Interface.ServerProtocol;
using NHapiTools.Base.Util;
using SuperSocket.Facility.Protocol;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Interface.ServerProtocol
{
    public class MLLPBeginEndMarkReceiveFilter : BeginEndMarkReceiveFilter<HL7Request>
    {
        private readonly static byte[] beginMark = new byte[] { 11 };
        private readonly static byte[] endMark = new byte[] { 28, 13 };
        private IHL7Protocol m_Protocol;

        public MLLPBeginEndMarkReceiveFilter() : base(beginMark, endMark)
        {
            m_Protocol = new BaseHL7Protocol();
        }

        public MLLPBeginEndMarkReceiveFilter(IHL7Protocol protocol, byte[] begin, byte[] end) : base(begin, end)
        {
            this.m_Protocol = protocol;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override HL7Request Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
        {
            return base.Filter(readBuffer, offset, length, toBeCopied, out rest);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected override HL7Request ProcessMatchedRequest(byte[] readBuffer, int offset, int length)
        {
            byte[] msg = new byte[length];
            Array.Copy(readBuffer, offset, msg, offset, length);


            string message = Encoding.UTF8.GetString(msg);
            StringBuilder sb = new StringBuilder(message);
            MLLP.StripMLLPContainer(sb);


            ParserResult result = m_Protocol.Parse(sb.ToString());
        

            HL7Request request = new HL7Request();
            if (result.IsAccepted)
            {
                request.Request = result.ParsedMessage;
                request.Acknowledgment = result.Acknowledge;

                request.Key = "V" + result.ParsedMessage.MessageVersion.Replace(".", "");
                request.Key += result.ParsedMessage.MessageID;
            }
            return request;
        }
    }
}
