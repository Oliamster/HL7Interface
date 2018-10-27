using HL7api.Model;
using Hl7Interface.ServerProtocol;
using HL7Interface.ClientProtocol;
using HL7Interface.Configuration;
using HL7Interface.ServerProtocol;
using SuperSocket.ClientEngine;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHapiTools.Base.Util;

namespace HL7Interface
{
    public class BaseHL7Interface : IHL7Interface
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private HL7Server m_HL7Server;
        private BaseHL7Protocol m_Protocol;
        private ConcurrentQueue<IHL7Message> incomingAcknowledgment;
        private object incomingAckLock = new object();

        public void Stop()
        {
            m_HL7Server.Stop();
        }

        private AutoResetEvent ackReceivedSignal = new AutoResetEvent(false);

        public BaseHL7Interface()
        {
            incomingAcknowledgment = new ConcurrentQueue<IHL7Message>();
            Client = new EasyClient();
            m_Protocol = new BaseHL7Protocol();
            m_HL7Server = new HL7Server();

        }


        public virtual string Name => this.GetType().Name;

        public EasyClient Client { get;  }

        IHL7Protocol IHL7Interface.Protocol => throw new NotImplementedException();

        string IHL7Interface.Name => throw new NotImplementedException();

        public ServerState State => (ServerState)m_HL7Server.State;

        async Task<bool> IHL7Interface.ConnectAsync(EndPoint remoteEndPoint)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            bool ret = false;
            var t = await Task.Factory.StartNew(async () =>
            {
                while (!ret && !cts.Token.IsCancellationRequested)
                {
                    ret = await Client.ConnectAsync(remoteEndPoint);
                }
            }, cts.Token);
            t.Wait(5000);
            if (ret)
                return ret;
            else cts.Cancel();
            Task.WaitAll(new Task[] { t });
               throw new OperationCanceledException("Unable to connect to the remote endpoint:!");
        }

       public virtual bool Initialize()
       {
            IBootstrap bootstrap = BootstrapFactory.CreateBootstrap();
            if (!bootstrap.Initialize())
                return false;
            IWorkItem item = bootstrap.AppServers.FirstOrDefault(server => server.Name == Name);
            m_HL7Server = item as HL7Server;
            if (m_HL7Server == null)
                return false;
            //_hl7Server.Logger.Debug("Server Initialized"); TODO: Exploit the AppServer Logger
            log.Debug("the Server side is initializing");

            HL7SocketServiceConfig config = bootstrap.Config as HL7SocketServiceConfig;
            if (config == null && config.ProtocolConfig == null)
                return false;
            m_Protocol.Config = config.ProtocolConfig;

            log.Debug("the Client side is initializing");
            Client.Initialize(new ReceiverFilter(m_Protocol), (request) => {
                lock (incomingAckLock)
                    incomingAcknowledgment.Enqueue(request.RequestMessage);
                ackReceivedSignal.Set();
            });
            return true;
        }

        Task<HL7Request> IHL7Interface.SendHL7MessageAsync(IHL7Message message)
        {
            Client.Send(Encoding.UTF8.GetBytes(MLLP.CreateMLLPMessage(message.Encode())));

            throw new NotImplementedException();   
        }

        public HL7Server HL7Server
        {
            get { return m_HL7Server; }
        }

        public bool Start()
        {
            return m_HL7Server.Start();
        }

        bool IHL7Interface.Stop()
        {
            throw new NotImplementedException();
        }
    }
}

