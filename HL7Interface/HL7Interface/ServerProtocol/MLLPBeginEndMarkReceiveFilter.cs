
using SuperSocket.SocketBase.Protocol;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Interface.ServerProtocol
{
    public class MLLPBeginEndMarkReceiveFilter : IReceiveFilter<HL7Request>
    {
        public int LeftBufferSize => throw new NotImplementedException();

        public IReceiveFilter<HL7Request> NextReceiveFilter => throw new NotImplementedException();

        public FilterState State => throw new NotImplementedException();

        public HL7Request Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
