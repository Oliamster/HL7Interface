
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
        private readonly static byte[] beginMark = new byte[] { 11 };
        private readonly static byte[] endMark = new byte[] { 28, 13}; 
        private IHL7Protocol m_Protocol;

        public ReceiverFilter(IHL7Protocol protocol) : this(protocol, beginMark, endMark)
        {
            
        }

        public ReceiverFilter(IHL7Protocol protocol, byte[] begin, byte[] end) : base (begin,  end)
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
            string message = Encoding.UTF8.GetString(data);

            PackageInfo package = new PackageInfo();

            ParserResult result = m_Protocol.Parse(message);
            if (result.IsAccepted)
            {
                package.RequestMessage = result.ParsedMessage;
                package.Key = result.ParsedMessage.ControlID;
            }
            else
            {
                package.OriginalRequest = message;
            }
                
            return package;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

}
