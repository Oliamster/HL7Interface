
using HL7api.Parser;
using SuperSocket.ProtoBase;
using System;
using System.Text;

namespace HL7Interface.Tests.Protobase
{
    /// <summary>
    /// The Easy client Receiver filter
    /// </summary>
    public class TestProtoBaseBeginEndMarkReceiverFilter : BeginEndMarkReceiveFilterTest<StringPackageInfo>
    {
        private  static byte[] beginMark = new byte[] { 11 };
        private  static byte[] endMark = new byte[] { 28, 13 };
     
        public TestProtoBaseBeginEndMarkReceiverFilter() 
            : this(beginMark, endMark)
        {
            
        }

        public TestProtoBaseBeginEndMarkReceiverFilter(byte[] begin, byte[] end) : base (begin,  end)
        {
            beginMark = begin;
            endMark = end;
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override StringPackageInfo Filter(BufferList data, out int rest)
        {
            return base.Filter(data, out rest);
        }

        public override StringPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            byte[] data = new byte[bufferStream.Length];
            bufferStream.Read(data, 0, Convert.ToInt32(bufferStream.Length));
            string message = Encoding.ASCII.GetString(data);
            return ResolvePackage(data);
        }
        public StringPackageInfo ResolvePackage(byte[] buffer)
        {
            //PackageInfo package = new PackageInfo();

            //ParserResult result = m_Protocol.Parse(buffer);

            //if (result.MessageAccepted)
            //{
            //    package.Request = result.ParsedMessage;
            //    package.Key = result.ParsedMessage.MessageID;
            //    package.Acknowledgment = result.Acknowledge;
            //}
            //return package;




            string message = Encoding.ASCII.GetString(buffer);

            if (ValidateBeginEndFilteredMarkMessage(message))
                StripBeginEndMarkContainer(ref message);

            StringPackageInfo package = new StringPackageInfo();

            ParserResult result = m_Protocol.Parse(buffer);
            if (result.MessageAccepted)
            {
                package.Request = result.ParsedMessage;
                package.Key = result.ParsedMessage.MessageID;
            }
            else
            {
                package.OriginalRequest = message;
            }

            return package;
        }



            static void StripBeginEndMarkContainer(ref string message)
        {
            StringBuilder sb = new StringBuilder(message);
            if (ValidateBeginEndFilteredMarkMessage(message) == true)
            {
                // Strip the message of the begin and end mark container characters                
                sb.Remove(0, beginMark.Length);
                sb.Remove(sb.Length - endMark.Length, endMark.Length);
                message = sb.ToString();
            }
        }

        public static bool ValidateBeginEndFilteredMarkMessage(string message)
        {
            StringBuilder sb = new StringBuilder(message);

            //Should I accept blank message? if no change to "<="
            if (sb.Length < beginMark.Length + endMark.Length)
                return false;

            try
            {
                for (int i = 0; i < beginMark.Length; i++)
                {
                    if (message[i] != (char)beginMark[i])
                        return false;
                }
                for (int i = 0; i < endMark.Length; i++)
                {
                    if (message[message.Length - endMark.Length + i] != (char)endMark[i])
                        return false;
                }
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
            
            return true;
        }
    }
}
