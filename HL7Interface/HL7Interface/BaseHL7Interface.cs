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
using HL7api.Parser;

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

        public IHL7Protocol Protocol
        {
            get
            {
                return m_Protocol;

            }
        }

        string IHL7Interface.Name => throw new NotImplementedException();

        public ServerState State => (ServerState)m_HL7Server.State;

        public async Task<bool> ConnectAsync(EndPoint remoteEndPoint)
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


        public virtual bool Initialize(HL7Server server, IHL7Protocol protocol)
        {
            if (protocol.Config == null)
                throw new ArgumentNullException("The configuration proprty is missing for this protocol");
            m_Protocol = protocol as BaseHL7Protocol;

            if(m_Protocol == null)
                return false;

            m_HL7Server = server;

            Client.Initialize(new ReceiverFilter(m_Protocol), (request) => {
                lock (incomingAckLock)
                    incomingAcknowledgment.Enqueue(request.RequestMessage);
                ackReceivedSignal.Set();
            });

            return true;
        }

        public Task<HL7Request> SendHL7MessageAsync(IHL7Message message)
        {
            return Task.Run( async () =>
            {
                Client.Send(Encoding.UTF8.GetBytes(MLLP.CreateMLLPMessage(message.Encode())));

                IHL7Message ack = null;

                if (Protocol.Config.IsAckRequired)
                {
                    ack = await WaitForAcknowlwdgment(message, new CancellationToken());
                }

                return new HL7Request()
                {
                    RequestMessage = message,
                    Acknowledgment = ack,
                };
            });
        }

        public HL7Server HL7Server
        {
            get { return m_HL7Server; }
        }

        public bool Start()
        {
            return m_HL7Server.Start();
        }

        private Task<IHL7Message> WaitForAcknowlwdgment(IHL7Message request, CancellationToken token)
        {
            IHL7Message ack = null;
            return Task.Run(() =>
            {
                ackReceivedSignal.WaitOne();
                lock (incomingAckLock)
                {
                    if (incomingAcknowledgment.TryPeek(out ack))
                    {
                        if (HL7Parser.IsAckForRequest(request, ack))
                        {
                            if (incomingAcknowledgment.TryDequeue(out ack))
                            {
                                return ack;
                            }
                        }
                    }
                }
                return null;
            });
        }
        
    }
}

