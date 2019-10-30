﻿using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using NHapiTools.Base.Util;
using HL7api.Model;
using Hl7Interface.ServerProtocol;
using HL7Interface.ClientProtocol;
using HL7Interface.Configuration;
using HL7Interface.ServerProtocol;
using SuperSocket.SocketBase;
using SuperSocket.SocketEngine;
using SuperSocket.ClientEngine;
using SuperSocket.SocketBase.Config;

namespace HL7Interface
{
    /// <summary>
    /// The base HL7 Interface for use in the application 
    /// </summary>
    public class HL7InterfaceBase : IHL7Interface
    {
        #region Private Properties
        private HL7Server m_HL7Server;
        private EasyClient m_EasyClient;
        private HL7ProtocolBase m_HL7Protocol;
        private BlockingCollection<HL7Request> m_OutgoingRequests;
        private EndPoint m_LocalEndpoint;
        #endregion

        #region Constructor

        /// <summary>
        /// Create a new Instance of HL7Interface
        /// </summary>
        public HL7InterfaceBase()
        {
            m_OutgoingRequests = new BlockingCollection<HL7Request>();
            m_EasyClient = new SuperSocket.ClientEngine.EasyClient();
            m_HL7Protocol = new HL7ProtocolBase();
            m_HL7Server = new HL7Server();
        }
        #endregion

        #region Events
        public event RequestHandler<HL7Session, HL7Request> NewRequestReceived
        {
            add { m_HL7Server.NewRequestReceived += value; }
            remove { m_HL7Server.NewRequestReceived -= value; }
        }

        public event SessionHandler<HL7Session> NewSessionConnected
        {
            add { m_HL7Server.NewSessionConnected += value; }
            remove { m_HL7Server.NewSessionConnected -= value; }
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// The interface name
        /// </summary>
        public virtual string Name => this.GetType().Name;
        #endregion

        /// <summary>
        /// The protocol for use in this interface
        /// </summary>
        public virtual IHL7Protocol Protocol
        {
            get
            {
                return m_HL7Protocol;
            }
        }

        /// <summary>
        /// The interface state
        /// </summary>
        public ServerState State => (ServerState)m_HL7Server.State;

        /// <summary>
        /// Connect to the remote end point
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <returns></returns>
        public async Task<bool> ConnectAsync(EndPoint remoteEndPoint)
        {
            if (remoteEndPoint.Equals(m_LocalEndpoint))
            {
                return false;       
            }
               
            CancellationTokenSource cts = new CancellationTokenSource();

            bool ret = false;

            try
            {
                var t = await Task.Factory.StartNew(async () =>
                {
                    while (!ret)
                    {
                        cts.Token.ThrowIfCancellationRequested();

                        ret = await m_EasyClient.ConnectAsync(remoteEndPoint);
                    }
                }, cts.Token);

                if (!t.Wait(Protocol.Config.ConnectionTimeout)) cts.Cancel();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Unable to connect to the remote endpoint:!");
            }
            return ret;
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

            m_HL7Server.Logger.Debug("the Server side is initializing");

            HL7SocketServiceConfig config = bootstrap.Config as HL7SocketServiceConfig;

            if (config == null && config.ProtocolConfig == null)
                return false;

            m_HL7Protocol.Config = config.ProtocolConfig;


            return Initialize(m_HL7Server, m_HL7Protocol);
        }

        public virtual bool Initialize(HL7Server server, IHL7Protocol protocol)
        {
            server.Logger.Debug("the Client side is initializing");

            if (protocol.Config == null)
                throw new ArgumentNullException("The configuration proprty is missing for this protocol");

            m_HL7Protocol = protocol as HL7ProtocolBase;

            if(m_HL7Protocol == null)
                return false;

            m_HL7Server = server;

            m_LocalEndpoint = new IPEndPoint(IPAddress.Parse(m_HL7Server.Config.Ip), m_HL7Server.Config.Port);

            m_EasyClient.Initialize(new ReceiverFilter(m_HL7Protocol), (request) =>
            {
                if (request.Request.IsAcknowledge)
                {
                    ProcessIncomingAck(request.Request);
                }
                else
                {
                    ProcessIncomingRequest(request);
                }
            });

            NewRequestReceived += OnNewRequestReceived;

            return true;
        }

        public virtual bool Initialize(IProtocolConfig protocolConfig, IServerConfig serverConfig, HL7ProtocolBase hL7Protocol = null)
        {
            if (!m_HL7Server.Setup(serverConfig))
                return false;

            if (hL7Protocol != null) m_HL7Protocol = hL7Protocol;

            m_HL7Protocol.Config = protocolConfig;

            m_LocalEndpoint = new IPEndPoint(IPAddress.Parse(m_HL7Server.Config.Ip), m_HL7Server.Config.Port);

            m_EasyClient.Initialize(new ReceiverFilter(m_HL7Protocol), (request) =>
            {
                if (request.Request.IsAcknowledge)
                {
                    ProcessIncomingAck(request.Request);
                }
                else
                {
                    ProcessIncomingRequest(request);
                }
            });

            NewRequestReceived += OnNewRequestReceived;

            return true;
        }

        private void OnNewRequestReceived(HL7Session session, HL7Request requestInfo)
        {
            ProcessIncomingRequest(requestInfo);
        }

        public Task<HL7Request> SendHL7MessageAsync(IHL7Message message)
        {
            HL7Request hl7Request = new HL7Request()
            {
                Request = message,
                ResponseReceivedEvent = new AutoResetEvent(false),
                AckReceivedEvent = new AutoResetEvent(false),
                RequestCompletedEvent = new AutoResetEvent(false),
                RequestCancellationToken = new CancellationTokenSource()
            };

            m_OutgoingRequests.Add(hl7Request);

            TaskCompletionSource<HL7Request> senderTaskCompletionSource = new TaskCompletionSource<HL7Request>();


            hl7Request.SenderTask = Task.Run(() =>
            {
                try
                {
                    hl7Request.RequestCancellationToken.Token.ThrowIfCancellationRequested();

                    m_EasyClient.Send(Encoding.ASCII.GetBytes(MLLP.CreateMLLPMessage(message.Encode())));

                    if (!Protocol.Config.IsAckRequired)
                        return hl7Request;

                    if (!m_OutgoingRequests.Contains(hl7Request))
                    {
                        throw new OperationCanceledException("");
                    }

                    if (hl7Request.RequestCancellationToken.Token.IsCancellationRequested)
                        throw new OperationCanceledException(hl7Request.RequestCancellationToken.Token);

                

                    hl7Request.RequestCancellationToken.Token.ThrowIfCancellationRequested();

                    if (!hl7Request.AckReceivedEvent.Wait(Protocol.Config.AckTimeout + 10000, hl7Request.RequestCancellationToken.Token))
                    {
                        
                    }
                    else
                    {

                    }

                    if (!Protocol.Config.IsResponseRequired)
                        return hl7Request;


                    if (!hl7Request.ResponseReceivedEvent.Wait(Protocol.Config.ResponseTimeout + 20000, hl7Request.RequestCancellationToken.Token))
                    {

                    }
                    else
                    {

                    }
                    return hl7Request;
                }
                catch (Exception) 
                {
                    hl7Request.RequestCompletedEvent.Set();
                    return hl7Request;
                }
            }, hl7Request.RequestCancellationToken.Token);

            return hl7Request.SenderTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ack"></param>
        /// <returns></returns>
        private void ProcessIncomingAck(IHL7Message ack)
        {
            HL7Request req = null;

            var v = m_OutgoingRequests.Where((request, b) =>
            {
                if (ack.IsAckForRequest(request.Request) && request.Acknowledgment == null)
                {
                    request.Acknowledgment = ack as IHL7Message;
                    
                    return true;
                }
                else return false;

            }).ToList();

            if (v.Count() == 0)
            {
                string log = "Unexpected ack received or ack received to late";
                m_HL7Server.Logger.Error(log);
            }
            else if (v.Count() > 1)
            {
                string log = "each ack should be bount to a single request";
                throw new HL7InterfaceException(log);
            }
            else
            {
                req = v.FirstOrDefault();

                req.AckReceivedEvent.Set();
            }
        }
    

        public HL7Server HL7Server
        {
            get { return m_HL7Server; }
        }

        public bool Start()
        {
            return m_HL7Server.Start();
        }

        private void ProcessIncomingRequest(HL7Request response)
        {
            HL7Request req = null;

            var v = m_OutgoingRequests.Where((request, b) =>
            {
                if (response.Request.IsResponseForRequest(request.Request))
                {
                    request.Response = response.Request;
                    return true;
                }
                else return false;

            }).ToList();

           
            if (v.Count() == 0)
            {
                m_HL7Server.Logger.Error("Unexpected response received");
            }                 
            else
            {
                req = v.FirstOrDefault();

                req.ResponseReceivedEvent.Set();
            }
        }

        public void Stop()
        {
            HL7Request item;

            while (m_OutgoingRequests.TryTake(out item))
            {
                if (!item.SenderTask.IsCompleted)
                {
                    item.RequestCancellationToken.Cancel();

                    item.RequestCompletedEvent.WaitOne();
                }
            }

            if (m_EasyClient.IsConnected)
                m_EasyClient.Close().Wait();

            m_HL7Server.Stop();
        }
    }
}
