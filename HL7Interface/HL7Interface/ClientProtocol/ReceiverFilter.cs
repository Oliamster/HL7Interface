
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
        public ReceiverFilter(byte[] beginMark, byte[] endMark) : base(beginMark, endMark)
        {
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
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }

}
