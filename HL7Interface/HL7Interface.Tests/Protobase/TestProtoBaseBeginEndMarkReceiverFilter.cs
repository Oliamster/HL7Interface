
using HL7api.Parser;
using SuperSocket.ProtoBase;
using System;
using System.Text;

namespace HL7Interface.Tests.Protobase
{
    /// <summary>
    /// The Easy client Receiver filter
    /// </summary>
    public class TestProtoBaseBeginEndMarkReceiverFilter : BeginEndMarkReceiveFilter<TestProtobasePackageInfo>
    {
        private readonly static byte[] beginMark = Encoding.ASCII.GetBytes("|");
        private readonly static byte[] endMark = Encoding.ASCII.GetBytes("||");


        public TestProtoBaseBeginEndMarkReceiverFilter()
            : this(beginMark, endMark)
        {

        }

        public TestProtoBaseBeginEndMarkReceiverFilter(byte[] beginMark, byte[] endMark)
        : base(beginMark, endMark)
        {
        }


        public override TestProtobasePackageInfo Filter(BufferList data, out int rest)
        {
            return base.Filter(data, out rest);
        }

        public override TestProtobasePackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            byte[] data = new byte[bufferStream.Length];

            bufferStream.Read(data, 0, Convert.ToInt32(bufferStream.Length));

            string message = Encoding.ASCII.GetString(data);

            return new TestProtobasePackageInfo()
            {
                OriginalRequest = message.TrimStart(Encoding.ASCII.GetChars(beginMark)).TrimEnd(Encoding.ASCII.GetChars(endMark))
            };
        }
    }
}
