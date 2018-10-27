using HL7Interface;
using SuperSocket.ProtoBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Interface.ClientProtocol
{
    public class PackageInfo :  HL7Request, SuperSocket.ProtoBase.IPackageInfo
    {
        public string OriginalRequest { get; set; }
    }
}
