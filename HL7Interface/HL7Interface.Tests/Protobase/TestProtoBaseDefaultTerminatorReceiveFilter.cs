using SuperSocket.ProtoBase;
using System;
using System.Text;

namespace HL7Interface.Tests.Protobase
{
    public class TestProtoBaseDefaultTerminatorReceiverFilter : SuperSocket.ProtoBase.TerminatorReceiveFilter<StringPackageInfo>
    {
        public TestProtoBaseDefaultTerminatorReceiverFilter()
            : base(Encoding.ASCII.GetBytes(Environment.NewLine))
        {

        }

        public TestProtoBaseDefaultTerminatorReceiverFilter(byte[] terminatorMark)
            : base(terminatorMark)
        {

        }

        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            byte[] data = new byte[bufferStream.Length];
            bufferStream.Read(data, 0, Convert.ToInt32(bufferStream.Length));

            string message = Encoding.ASCII.GetString(data);

            BasicStringParser bsp = new BasicStringParser(Environment.NewLine, Environment.NewLine);

            StringPackageInfo package = new StringPackageInfo(message, bsp);

            return package;
        }
    }
}
