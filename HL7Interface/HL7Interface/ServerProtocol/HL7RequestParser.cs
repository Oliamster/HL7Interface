
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Interface.ServerProtocol
{
    public class HL7RequestParser : IRequestInfoParser<HL7Request>
    {
        public HL7Request ParseRequestInfo(string source)
        {
            throw new NotImplementedException();
        }
    }
}

