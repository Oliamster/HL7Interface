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
    public class BaseHL7Interface : IHL7Interface
    {
        #region Private Properties
        private HL7Server m_HL7Server;
        private BaseHL7Protocol m_Protocol;
        private ConcurrentQueue<IHL7Message> incomingAcknowledgmentQueue;
        private ConcurrentStack<IHL7Message> incomingMessageQueue;
        private object AckQueueLock = new object();
        private object responseQueueLock = new object();
        private AutoResetEvent ackReceivedSignal = new AutoResetEvent(false);
        private AutoResetEvent responseReceivedSignal = new AutoResetEvent(false);
        #endregion

        #region Constructor
        public BaseHL7Interface()
        {
            incomingAcknowledgmentQueue = new ConcurrentQueue<IHL7Message>();
            incomingMessageQueue = new ConcurrentStack<IHL7Message>();
            Client = new EasyClient();
            m_Protocol = new BaseHL7Protocol();
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

            log.Debug("the Client side is initializing");

            Client.Initialize(new ReceiverFilter(m_Protocol), (request) =>
            {
                if (request.Request.IsAcknowledge)
                {
                    lock (responseQueueLock)
                        incomingAcknowledgmentQueue.Enqueue(request.Request);
                    ackReceivedSignal.Set();
                }
                else
                {
                    lock (responseQueueLock)
                        incomingMessageQueue.Push(request.Request);
                    responseReceivedSignal.Set();
                }
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
                if(request.Request.IsAcknowledge)
                {
                    lock (responseQueueLock)
                        incomingAcknowledgmentQueue.Enqueue(request.Request);
                    ackReceivedSignal.Set();
                }
                else
                {
                    lock (responseQueueLock)
                        incomingMessageQueue.Push(request.Request);
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
                            foreach (var item in incomingMessageQueue)
                            {
                                if (item.IsResponseForRequest(request))
                                    return item;
                            }
                            //if (incomingMessageQueue.TryPeek(out response))
                            //{
                            //    if (HL7Parser.IsResponseForRequest(request, response))
                            //    {
                            //        if (incomingMessageQueue.TryPop(out response))
                            //        {
                            //            return response;
                            //        }
                            //    }
                            //}
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
                    if (incomingAcknowledgmentQueue.TryPeek(out ack))
                    {
                        if (HL7Parser.IsAckForRequest(request, ack))
                        {
                            if (incomingAcknowledgmentQueue.TryDequeue(out ack))
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
