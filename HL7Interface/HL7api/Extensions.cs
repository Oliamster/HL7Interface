using NHapi.Base.Model;
using NHapi.Base.Util;

namespace HL7api
{
    public static class Extensions
    {
        #region NHapi Extension Methods
        public static string GetMessageType(this IMessage message)
        {
            ISegment msh = (ISegment)message.GetStructure("MSH");
            string messageType = $"{Terser.Get(msh, 9, 0, 1, 1)}" +
                $"_{Terser.Get(msh, 9, 0, 2, 1)}";
            return messageType;
        }
        #endregion
    }
}
