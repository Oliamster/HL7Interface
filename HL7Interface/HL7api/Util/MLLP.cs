using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7api.Util
{
    public class MLLP
    {
        public static bool IsStartCharacter(char start)
        {
            return NHapiTools.Base.Util.MLLP.IsStartCharacter(start);
        }
        public static void StripMLLPContainer(ref string message)
        {
            StringBuilder sb = new StringBuilder(message);
            if (NHapiTools.Base.Util.MLLP.ValidateMLLPMessage(sb) == true)
            {
                NHapiTools.Base.Util.MLLP.StripMLLPContainer(sb);
                message = sb.ToString();
            }
        }

        public static bool ValidateMLLPMessage(string message)
        {
            StringBuilder sb = new StringBuilder(message);
            return NHapiTools.Base.Util.MLLP.ValidateMLLPMessage(sb);
        }
        public static string CreateMLLPMessage(string p)
        {
            return NHapiTools.Base.Util.MLLP.CreateMLLPMessage(p);
        }
    }
}
