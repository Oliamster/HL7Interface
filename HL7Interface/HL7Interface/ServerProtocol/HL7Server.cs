using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;
using NHapiTools.Base.Util;

namespace HL7Interface.ServerProtocol
{
    /// <summary>
    /// The HL7 MLLP server
    /// </summary>
    public class HL7Server : AppServer<HL7Session, HL7Request>
    {
        public HL7Server()
            : base(new DefaultReceiveFilterFactory<MLLPBeginEndMarkReceiveFilter, HL7Request>())
        {
            
        }

        public override IReceiveFilterFactory<HL7Request> ReceiveFilterFactory
        {
            get => base.ReceiveFilterFactory;
            protected set => base.ReceiveFilterFactory = value;
        }

        public override int SessionCount => base.SessionCount;

        public override IEnumerable<HL7Session> GetAllSessions()
        {
            return base.GetAllSessions();
        }

        public override HL7Session GetSessionByID(string sessionID)
        {
            return base.GetSessionByID(sessionID);
        }

        public override IEnumerable<HL7Session> GetSessions(Func<HL7Session, bool> critera)
        {
            return base.GetSessions(critera);
        }

        public override bool Start()
        {
            return base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        protected override HL7Session CreateAppSession(ISocketSession socketSession)
        {
            return base.CreateAppSession(socketSession);
        }

        protected override ILog CreateLogger(string loggerName)
        {
            return base.CreateLogger(loggerName);
        }

        /// <summary>
        /// Execute the command convayed by the HL7Request
        /// </summary>
        /// <param name="session"></param>
        /// <param name="requestInfo"></param>
        protected override void ExecuteCommand(HL7Session session, HL7Request requestInfo)
        {
            try
            {
                Logger.Debug($"Message received: {requestInfo.Request.Encode()}");

                var ack = MLLP.CreateMLLPMessage(requestInfo.Acknowledgment.Encode());

                session.Send(ack);

                base.ExecuteCommand(session, requestInfo);
                Logger.Debug($"The command {requestInfo.Request.MessageID} has been executed");
            }
            catch (Exception e)
            {
                Logger.Debug($"ERROR: {e.Message}");
                throw e;
            }
        }

        protected override X509Certificate GetCertificate(ICertificateConfig certificate)
        {
            return base.GetCertificate(certificate);
        }

        protected override void OnNewSessionConnected(HL7Session session)
        {
            //session.Send(MLLP.CreateMLLPMessage((new GeneralAcknowledgment()).Encode()));
            base.OnNewSessionConnected(session);
        }

        protected override void OnServerStatusCollected(StatusInfoCollection bootstrapStatus, StatusInfoCollection serverStatus)
        {
            base.OnServerStatusCollected(bootstrapStatus, serverStatus);
        }

        protected override void OnSessionClosed(HL7Session session, CloseReason reason)
        {
            base.OnSessionClosed(session, reason);
        }

        protected override void OnStarted()
        {
            base.OnStarted();
        }

        [Obsolete]
        protected override void OnStartup()
        {
            base.OnStartup();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }

        protected override void OnSystemMessageReceived(string messageType, object messageData)
        {
            base.OnSystemMessageReceived(messageType, messageData);
        }

        protected override bool RegisterSession(string sessionID, HL7Session appSession)
        {
            return base.RegisterSession(sessionID, appSession);
        }

        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {
            return base.Setup(rootConfig, config);
        }

        protected override bool SetupCommandLoaders(List<ICommandLoader<ICommand<HL7Session, HL7Request>>> commandLoaders)
        {
            return base.SetupCommandLoaders(commandLoaders);
        }

        protected override bool SetupCommands(Dictionary<string, ICommand<HL7Session, HL7Request>> discoveredCommands)
        {
            return base.SetupCommands(discoveredCommands);
        }

        protected override void UpdateServerStatus(StatusInfoCollection serverStatus)
        {
            base.UpdateServerStatus(serverStatus);
        }

        protected override bool ValidateClientCertificate(HL7Session session, object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return base.ValidateClientCertificate(session, sender, certificate, chain, sslPolicyErrors);
        }
    }
}
