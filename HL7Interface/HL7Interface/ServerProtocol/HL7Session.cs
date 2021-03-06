﻿using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Interface.ServerProtocol
{
    public class HL7Session : AppSession<HL7Session, HL7Request>
    {
        public override AppServerBase<HL7Session, HL7Request> AppServer => base.AppServer;

        public override void Close(CloseReason reason)
        {
            base.Close(reason);
        }

        public override void Close()
        {
            base.Close();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void Initialize(IAppServer<HL7Session, HL7Request> appServer, ISocketSession socketSession)
        {
            base.Initialize(appServer, socketSession);
        }

        public override void Send(byte[] data, int offset, int length)
        {
            base.Send(data, offset, length);
        }

        public override void Send(ArraySegment<byte> segment)
        {
            base.Send(segment);
        }

        public override void Send(IList<ArraySegment<byte>> segments)
        {
            base.Send(segments);
        }

        public override void Send(string message)
        {
            byte[] bytesToSend = Encoding.ASCII.GetBytes(message);
            base.Send(bytesToSend, 0, bytesToSend.Length);
        }

        public override void Send(string message, params object[] paramValues)
        {
            base.Send(message, paramValues);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override bool TrySend(string message)
        {
            return base.TrySend(message);
        }

        public override bool TrySend(byte[] data, int offset, int length)
        {
            return base.TrySend(data, offset, length);
        }

        public override bool TrySend(ArraySegment<byte> segment)
        {
            return base.TrySend(segment);
        }

        public override bool TrySend(IList<ArraySegment<byte>> segments)
        {
            return base.TrySend(segments);
        }

        protected override int GetMaxRequestLength()
        {
            return base.GetMaxRequestLength();
        }

        protected override void HandleException(Exception e)
        {
            throw e;
        }

        protected override void HandleUnknownRequest(HL7Request requestInfo)
        {
            base.HandleUnknownRequest(requestInfo);
        }

        protected override void OnInit()
        {
            base.OnInit();
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            base.OnSessionClosed(reason);
        }

        protected override void OnSessionStarted()
        {
            base.OnSessionStarted();
        }

      
    }
}
