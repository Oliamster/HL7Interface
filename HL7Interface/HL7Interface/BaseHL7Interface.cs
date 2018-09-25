using NHapiPlus.Model;
using SuperSocket.ClientEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HL7Interface
{
    public class BaseHL7Interface : IHL7Interface
    {
        public virtual string Name => this.GetType().Name;

        public EasyClient Client { get;  }

        HL7Protocol IHL7Interface.Protocol => throw new NotImplementedException();

        string IHL7Interface.Name => throw new NotImplementedException();

        Task<bool> IHL7Interface.ConnectAsync(EndPoint remoteEndPoint)
        {
            throw new NotImplementedException();
        }

        bool IHL7Interface.Initialise()
        {
            throw new NotImplementedException();
        }

        Task<HL7Request> IHL7Interface.SendHL7MessageAsync(IHL7Message message)
        {
            throw new NotImplementedException();
        }

      

        bool IHL7Interface.Start()
        {
            throw new NotImplementedException();
        }

        bool IHL7Interface.Stop()
        {
            throw new NotImplementedException();
        }
    }
}
