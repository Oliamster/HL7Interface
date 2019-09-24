using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HL7api.Parser;
using Hl7Interface.ServerProtocol;
using HL7Interface.Tests.Protobase;
using NHapiTools.Base.Util;
using SuperSocket.Facility.Protocol;
using SuperSocket.ProtoBase;
using SuperSocket.SocketBase.Protocol;

namespace HL7Interface.Tests.Protocol
{


    public class TestProtoBaseTerminatorReceiverFilter : SuperSocket.ProtoBase.TerminatorReceiveFilter<TestProtobasePackageInfo>
    {
        public TestProtoBaseTerminatorReceiverFilter()
        : this(Encoding.ASCII.GetBytes("||"))
        {

        }

        public TestProtoBaseTerminatorReceiverFilter(byte[] terminatorMark)
            : base(terminatorMark)
        {

        }

        public override TestProtobasePackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            byte[] data = new byte[bufferStream.Length];
            bufferStream.Read(data, 0, Convert.ToInt32(bufferStream.Length));

            string message = Encoding.ASCII.GetString(data);

            TestProtobasePackageInfo package = new TestProtobasePackageInfo();

            package.OriginalRequest = message;

            return package;
        }

    }
}
