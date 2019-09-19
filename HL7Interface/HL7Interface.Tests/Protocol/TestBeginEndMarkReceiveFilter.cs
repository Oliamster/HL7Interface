
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
        private readonly static byte[] BeginMark = new byte[] { 0x5b, 0x5b };
        private readonly static byte[] EndMark = new byte[] { 0x5d, 0x5d };

        private BasicRequestInfoParser m_Parser = new BasicRequestInfoParser();

        public TestBeginEndMarkReceiveFilter()
            : this(BeginMark, EndMark)
        {

        }

        public TestBeginEndMarkReceiveFilter(byte[] beginMark, byte[] endMark)
        : base(beginMark, EndMark)
        {

        }



        protected override StringRequestInfo ProcessMatchedRequest(byte[] readBuffer, int offset, int length)
        {
            if (length < 20)
            {
                Console.WriteLine("Ignore request");
                return NullRequestInfo;
            }

            var line = Encoding.ASCII.GetString(readBuffer, offset, length);
            return m_Parser.ParseRequestInfo(line.Substring(2, line.Length - 4));
        }
    }

}
