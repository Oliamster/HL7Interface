using HL7api.Model;
using HL7Interface.ServerProtocol;
using SuperSocket.ClientEngine;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
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
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private HL7Server _hl7Server;

        public virtual string Name => this.GetType().Name;

        public EasyClient Client { get;  }

        HL7Protocol IHL7Interface.Protocol => throw new NotImplementedException();

        string IHL7Interface.Name => throw new NotImplementedException();

        Task<bool> IHL7Interface.ConnectAsync(EndPoint remoteEndPoint)
        {
            throw new NotImplementedException();
        }

       public virtual bool Initialise()
        {
            log.Debug("Server is initializing");
            IBootstrap bootstrap = BootstrapFactory.CreateBootstrap();
            if (!bootstrap.Initialize())
                return false;
            IWorkItem item = bootstrap.AppServers.FirstOrDefault(server => server.Name == Name);
            _hl7Server = item as HL7Server;
            if (_hl7Server == null)
                return false;
            return true;
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
