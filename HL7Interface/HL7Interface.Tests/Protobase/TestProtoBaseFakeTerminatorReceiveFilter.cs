using SuperSocket.ProtoBase;
using System.Text;

namespace HL7Interface.Tests.Protobase
{
    public class TestProtoBaseFakeTerminatorReceiverFilter : SuperSocket.ProtoBase.TerminatorReceiveFilter<TestProtobasePackageInfo>
    {
        public TestProtoBaseFakeTerminatorReceiverFilter()
            : base(Encoding.ASCII.GetBytes("\r\n"))
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
