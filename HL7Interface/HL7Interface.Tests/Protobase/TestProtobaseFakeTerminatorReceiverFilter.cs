using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Interface.Tests.Protobase
{
    public class TestFakeTerminatorReceiveFilter : SuperSocket.ProtoBase.TerminatorReceiveFilter<StringPackageInfo>
    {
        public TestFakeTerminatorReceiveFilter()
            : base(new byte[] { 0x01, 0x02 })
        {

        }

        public TestFakeTerminatorReceiveFilter(byte[] terminatorMark)
            : base(terminatorMark)
        {

        }


        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            return null;
        }
    }
}
