
using HL7api.Parser;
using Hl7Interface.ServerProtocol;
using NHapiTools.Base.Util;
using SuperSocket.Facility.Protocol;
using SuperSocket.ProtoBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Interface.Tests.Protocol
{


    public class TestProtobaseTerminatorReceiverFilter : SuperSocket.ProtoBase.TerminatorReceiveFilter<StringPackageInfo>
    {
        public TestProtobaseTerminatorReceiverFilter()
        : this(Encoding.ASCII.GetBytes("\r\n"))
        {

        }

        public TestProtobaseTerminatorReceiverFilter(byte[] terminatorMark)
            : base(terminatorMark)
        {

        }

        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            return null;
        }
    }
}
