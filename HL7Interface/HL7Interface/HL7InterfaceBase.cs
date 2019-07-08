using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using NHapiTools.Base.Util;
using HL7api.Parser;
using HL7api.Model;
using Hl7Interface.ServerProtocol;
using HL7Interface.ClientProtocol;
using HL7Interface.Configuration;
using HL7Interface.ServerProtocol;
using SuperSocket.ClientEngine;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;

namespace HL7Interface
{
    public class HL7InterfaceBase : IHL7Interface
    {
        #region Private Properties
        private HL7Server m_HL7Server;
        private HL7ProtocolBase m_Protocol;
        private ConcurrentQueue<IHL7Message> m_IncomingAcknowledgmentQueue;
        private ConcurrentStack<IHL7Message> m_IncomingMessageQueue;
        private object AckQueueLock = new object();
        private object responseQueueLock = new object();
        private AutoResetEvent ackReceivedSignal = new AutoResetEvent(false);
        private AutoResetEvent responseReceivedSignal = new AutoResetEvent(false);
        #endregion

        #region Constructor
        public HL7InterfaceBase()
        {
            m_IncomingAcknowledgmentQueue = new ConcurrentQueue<IHL7Message>();
            m_IncomingMessageQueue = new ConcurrentStack<IHL7Message>(); 
            Client = new EasyClient();
            m_Protocol = new HL7ProtocolBase();
            m_HL7Server = new HL7Server();
        }
        #endregion

        #region Public Properties
        public static readonly log4net.ILog log
           = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public virtual string Name => this.GetType().Name;

        public EasyClient Client { get; }
        #endregion

        public void Stop()
        {
            m_HL7Server.Stop();
        }
        

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

            //TODO: Exploit the AppServer Logger
            //_hl7Server.Logger.Debug("Server Initialized");
            log.Debug("the Server side is initializing");

            HL7SocketServiceConfig config = bootstrap.Config as HL7SocketServiceConfig;

            if (config == null && config.ProtocolConfig == null)
                return false;

            m_Protocol.Config = config.ProtocolConfig;


            return Initialize(m_HL7Server, m_Protocol);
        }

        public virtual bool Initialize(HL7Server server, IHL7Protocol protocol)
        {
            log.Debug("the Client side is initializing");

            if (protocol.Config == null)
                throw new ArgumentNullException("The configuration proprty is missing for this protocol");

            m_Protocol = protocol as HL7ProtocolBase;

            if(m_Protocol == null)
                return false;

            m_HL7Server = server;

            Client.Initialize(new ReceiverFilter(m_Protocol), (request) => {
                if(request.Request.IsAcknowledge)
                {
                    lock (responseQueueLock)
                        m_IncomingAcknowledgmentQueue.Enqueue(request.Request);
                    ackReceivedSignal.Set();
                }
                else
                {
                    lock (responseQueueLock)
                        m_IncomingMessageQueue.Push(request.Request);
                    responseReceivedSignal.Set();
                }
            });
            return true;
        }

        public Task<HL7Request> SendHL7MessageAsync(IHL7Message message)
        {
            return Task.Run( async () =>
            {
                Client.Send(Encoding.UTF8.GetBytes(MLLP.CreateMLLPMessage(message.Encode())));

                HL7Request hl7Request = new HL7Request()
                {
                    Request = message,
                };

                if (Protocol.Config.IsAckRequired)
                {
                    hl7Request.Acknowledgment = await WaitForAcknowlwdgment(message, new CancellationToken());
                }

                if (Protocol.Config.IsResponseRequired)
                {
                    hl7Request.Response = await WaitForResponse(message, new CancellationToken());
                }
                return hl7Request;
            });
        }

        /// <summary>
        /// Poll the queue until the response arrived or timeout
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private Task<IHL7Message> WaitForResponse(IHL7Message request, CancellationToken token)
        {
            IHL7Message response = null;
            return Task.Run(() =>
            {
                try
                {
                    do
                    {
                        responseReceivedSignal.WaitOne();
                        lock (responseQueueLock)
                        {
                            if (m_IncomingMessageQueue.TryPeek(out response))
                            {
                                if (response.IsResponseForRequest(request))
                                {
                                    if (m_IncomingMessageQueue.TryPop(out response))
                                    {
                                        return response;
                                    }
                                }
                            }
                        }
                    }
                    while (true);
                }
                catch (OperationCanceledException)
                {
                    response = null;
                    return response;
                }
            }, token);
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
                lock (AckQueueLock)
                {
                    if (m_IncomingAcknowledgmentQueue.TryPeek(out ack))
                    {
                        if (HL7Parser.IsAckForRequest(request, ack))
                        {
                            if (m_IncomingAcknowledgmentQueue.TryDequeue(out ack))
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
