using SuperSocket.ProtoBase;
using System;
using System.Text;

namespace HL7Interface.Tests.Protobase
{
    public class TestProtoBaseFakeTerminatorReceiverFilter : SuperSocket.ProtoBase.TerminatorReceiveFilter<TestProtobasePackageInfo>
    {
        public TestProtoBaseFakeTerminatorReceiverFilter()
            : base(Encoding.ASCII.GetBytes(Environment.NewLine))
        {

        }

        public TestProtoBaseFakeTerminatorReceiverFilter(byte[] terminatorMark)
            : base(terminatorMark)
        {

        }

        public override TestProtobasePackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            return null;
        }
    }
}
