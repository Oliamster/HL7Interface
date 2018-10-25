
using HL7api.Parser;
using HL7Interface.ServerProtocol;
using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SuperSocket.ProtoBase.Extensions;

namespace HL7Interface.ClientProtocol
{
    public class ReceiverFilter : BeginEndMarkReceiveFilter<PackageInfo>
    {
        private readonly static byte[] beginMark = new byte[] { 2 }; // HEX 0x02
        private readonly static byte[] endMark = new byte[] { 3 }; // HEX 0x03
        private IHL7Protocol m_Protocol;

        public ReceiverFilter(IHL7Protocol protocol) : base(beginMark, endMark)
        {
            this.m_Protocol = protocol;
        } 
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override PackageInfo Filter(BufferList data, out int rest)
        {
            return base.Filter(data, out rest);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override PackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            byte[] data = new byte[bufferStream.Length];
            bufferStream.Read(data, 0, Convert.ToInt32(bufferStream.Length));
            string message = Encoding.ASCII.GetString(data);

            PackageInfo package = new PackageInfo();

            ParserResult result = m_Protocol.Parse(message);
            if (result.IsAccepted)
                package.RequestMessage = result.ParsedMessage;
            package.Key = result.ParsedMessage.ControlID;
            return package;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

}
