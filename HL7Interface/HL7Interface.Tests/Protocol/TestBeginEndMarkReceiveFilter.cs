
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

namespace HL7Interface.Tests.Protocol
{
    class TestBeginEndMarkReceiveFilter : BeginEndMarkReceiveFilter<StringRequestInfo>
    {
        private readonly static byte[] BeginMark = Encoding.ASCII.GetBytes("#");
        private readonly static byte[] EndMark = Encoding.ASCII.GetBytes("##");
    

        private BasicRequestInfoParser m_Parser = new BasicRequestInfoParser();

        public TestBeginEndMarkReceiveFilter()
            : this(BeginMark, EndMark)
        {

        }

        public TestBeginEndMarkReceiveFilter(byte[] beginMark, byte[] endMark)
        : base(beginMark, endMark)
        {
        }

        public override StringRequestInfo Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
        {
            return base.Filter(readBuffer, offset, length, toBeCopied, out rest);
        }

        protected override StringRequestInfo ProcessMatchedRequest(byte[] readBuffer, int offset, int length)
        {
            string message = Encoding.ASCII.GetString(readBuffer);

            message = message.TrimStart(Encoding.ASCII.GetChars(BeginMark)).TrimEnd(Encoding.ASCII.GetChars(EndMark));

            return new StringRequestInfo(message, message, null);
        }
    }

}
