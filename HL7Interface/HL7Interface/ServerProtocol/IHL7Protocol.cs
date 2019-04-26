using HL7api.Model;
using HL7api.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Interface.ServerProtocol
{
    public interface IHL7Protocol : IProtocol
    {
        /// <summary>
        /// Parse
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        ParserResult Parse(string message);

        /// <summary>
        /// Encode
        /// </summary>
        /// <param name="hl7Message"></param>
        /// <returns></returns>
        byte[] Encode(IHL7Message hl7Message);
    }
}
