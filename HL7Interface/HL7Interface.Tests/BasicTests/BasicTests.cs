using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HL7api.Parser;
using HL7api.V251.Message;
using Hl7Interface.ServerProtocol;
using HL7Interface.ClientProtocol;
using HL7Interface.Configuration;
using HL7Interface.ServerProtocol;
using NHapiTools.Base.Util;
using NUnit.Framework;
using SuperSocket.ClientEngine;
using SuperSocket.SocketBase;
using SuperSocket.ProtoBase;
using SuperSocket.SocketBase.Protocol;
using HL7Interface.Tests.Protocol;
using HL7Interface.Tests.Protobase;
using System;
using System.Net.Sockets;
using System.Collections.Generic;

namespace HL7Interface.Tests.BasicTest
{
    [TestFixture]
    public class BasicTests : BasicTestsBase
    {
        [Test, Timeout(timeout)]
        public void A_TestAsyncTcpSession()
        {
            AppServer appServer = new AppServer();

            Assert.IsTrue(appServer.Setup("127.0.0.1", 50060));

            Assert.IsTrue(appServer.Start());

            appServer.NewRequestReceived += (s, e) =>
            {
                Assert.AreEqual("Hello!", e.Key);
                byte[] bytesToSend = Encoding.UTF8.GetBytes("Welcome!");
                s.Send(bytesToSend, 0, bytesToSend.Length);
            };

            AsyncTcpSession asyncTcpSession = new AsyncTcpSession();

            asyncTcpSession.ReceiveBufferSize = 8;

            AutoResetEvent sessionDataReceivedEvent = new AutoResetEvent(false);

            asyncTcpSession.DataReceived += (s, e) =>
            {
                string message = Encoding.ASCII.GetString(e.Data);
         
                Assert.AreEqual("Welcome!", message);

                sessionDataReceivedEvent.Set();
            };

            AutoResetEvent connectedEvent = new AutoResetEvent(false);

            asyncTcpSession.Connected += (s, e) =>
            {
                connectedEvent.Set();
            };

            asyncTcpSession.Connect(serverEndpoint);

            Assert.IsTrue(connectedEvent.WaitOne(timeout));

            Assert.IsTrue(asyncTcpSession.IsConnected);

            byte[] data = (Encoding.ASCII.GetBytes("Hello!" + Environment.NewLine));

            asyncTcpSession.Send(data, 0, data.Length);

            sessionDataReceivedEvent.WaitOne(timeout);

            asyncTcpSession.Close();

            appServer.Stop();
        }


        [Test, Timeout(timeout)]
        public async Task B_EasyClientConnection()
        {
            EasyClient easyClient = new EasyClient();
          
            easyClient.Initialize(new TestProtoBaseDefaultTerminatorReceiverFilter(), (p) =>
            {
                //do nothing
            });

            var ret = await easyClient.ConnectAsync(new DnsEndPoint("github.com", 443));

            Assert.True(ret);

            await easyClient.Close();
        }

        [Test, Timeout(timeout)]
         public async Task C_AppServerEasyClientNewSessionConnected()
         {
            AppServer appServer = new AppServer();

            Assert.IsTrue(appServer.Setup("127.0.0.1", 50060));

            Assert.IsTrue(appServer.Start());

            AutoResetEvent sessionConnectedEvent = new AutoResetEvent(false);

            appServer.NewSessionConnected += (s) =>
            {
                sessionConnectedEvent.Set();
            };

            EasyClient easyClient = new EasyClient();

            AutoResetEvent helloReceivedEvent = new AutoResetEvent(false);

            easyClient.Initialize(new TestProtoBaseDefaultTerminatorReceiverFilter(), (p) =>
            {
                //do nothing.
            });       

            bool connected = await easyClient.ConnectAsync(serverEndpoint);

            Assert.IsTrue(connected);

            sessionConnectedEvent.WaitOne(timeout);

            await easyClient.Close();

            appServer.Stop();
        }

        [Test, Timeout(timeout)]
        public async Task D_AppServerSendsWelcomeOnEasyClientNewSessionConnected()
        {
            AppServer appServer = new AppServer();

            Assert.IsTrue(appServer.Setup("127.0.0.1", 50060));

            Assert.IsTrue(appServer.Start());

            AutoResetEvent welcomeReceiveEvent = new AutoResetEvent(false);

            appServer.NewSessionConnected += (s) =>
            {
                s.Send("Welcome!");
            };

            EasyClient easyClient = new EasyClient();

            easyClient.Initialize(new TestProtoBaseDefaultTerminatorReceiverFilter(), (p) =>
            {
                Assert.AreEqual("Welcome!", p.Key);
                welcomeReceiveEvent.Set();
            });

            bool x = easyClient.ConnectAsync(serverEndpoint).Result;

            welcomeReceiveEvent.WaitOne();

            await easyClient.Close();

            appServer.Stop();
        }


        [Test, Timeout(timeout)]
        public async Task E_AppServerNewRequestReceivedFromEasyClient()
        {
            AppServer appServer = new AppServer();

            Assert.IsTrue(appServer.Setup("127.0.0.1", 50060));

            Assert.IsTrue(appServer.Start());

            AutoResetEvent newRequestReceivedEvent = new AutoResetEvent(false);

            appServer.NewRequestReceived += (s, e) =>
            {
                Assert.AreEqual("Hello!", e.Key);

                newRequestReceivedEvent.Set();
            };

            EasyClient easyClient = new EasyClient();

            easyClient.Initialize(new TestProtoBaseDefaultTerminatorReceiverFilter(), (p) =>
            {
                //do nothing
            });

            bool connected = easyClient.ConnectAsync(serverEndpoint).Result;

            Assert.IsTrue(connected);

            easyClient.Send(Encoding.ASCII.GetBytes("Hello!" + Environment.NewLine));

            newRequestReceivedEvent.WaitOne(timeout);

            await easyClient.Close();

            appServer.Stop();
        }


        [Test, Timeout(timeout)]
        public async Task F_AppSerserReplyToEasyClientNewRequestReceived()
        {
            AppServer appServer = new AppServer();

            Assert.IsTrue(appServer.Setup("127.0.0.1", 50060));

            Assert.IsTrue(appServer.Start());

            appServer.NewRequestReceived += (s, e) =>
            {
                Assert.AreEqual("Hello!", e.Key);

                s.Send("Hi There!");
            };

            EasyClient easyClient = new EasyClient();

            AutoResetEvent callbackEvent = new AutoResetEvent(false);

            easyClient.Initialize(new TestProtoBaseDefaultTerminatorReceiverFilter(), (p) =>
            {
                Assert.AreEqual("Hi There!", p.Key);

                callbackEvent.Set();
            });

            bool connected = easyClient.ConnectAsync(serverEndpoint).Result;

            Assert.IsTrue(connected);

            easyClient.Send(Encoding.ASCII.GetBytes("Hello!" + Environment.NewLine));

            callbackEvent.WaitOne(timeout);

            await easyClient.Close();

            appServer.Stop();

        }


        [Test, Timeout(timeout)]
        public async Task G_AppServerReceivesNewRequestFromEasyClient()
        {
            AppServer appServer = new AppServer();

            Assert.IsTrue(appServer.Setup("127.0.0.1", 50060));

            Assert.IsTrue(appServer.Start());

            AutoResetEvent newRequestReceivedEvent = new AutoResetEvent(false);

            appServer.NewRequestReceived += (s, e) =>
            {
                Assert.AreEqual("Welcome!", s.CurrentCommand);
                newRequestReceivedEvent.Set();
            };

            EasyClient easyClient = new EasyClient();

            easyClient.Initialize(new TestProtoBaseDefaultTerminatorReceiverFilter(), (p) =>
            {
                //do nothing
            });

            bool connected = await easyClient.ConnectAsync(serverEndpoint);

            Assert.IsTrue(connected);

            easyClient.Send(Encoding.ASCII.GetBytes("Welcome!" + Environment.NewLine));

            newRequestReceivedEvent.WaitOne(timeout);

            await easyClient.Close();

            appServer.Stop();
        }


        [Test, Timeout(timeout)]
        public async Task H_AppServerSendsWelcomeOnEasyClientNewSessionConnected()
        {
            AppServer appServer = new AppServer();

            Assert.IsTrue(appServer.Setup("127.0.0.1", 50060));

            Assert.IsTrue(appServer.Start());

            appServer.NewSessionConnected += (s) =>
            {
                ASCIIEncoding.ASCII.GetBytes("");
                s.Send("Welcome!");
            };

            EasyClient easyClient = new EasyClient();

            AutoResetEvent callbackEvent = new AutoResetEvent(false);

            easyClient.Initialize(new TestProtoBaseDefaultTerminatorReceiverFilter(), (p) =>
            {
                callbackEvent.Set();
            });

            bool connected = await easyClient.ConnectAsync(serverEndpoint);

            Assert.IsTrue(connected);

            callbackEvent.WaitOne(timeout);
        }


        [Test, Timeout(timeout)]
        public async Task I_TestEasyClientCallBack()
        {
            //byte[] begin = Encoding.ASCII.GetBytes("#");
            //byte[] end = Encoding.ASCII.GetBytes("##");

            //var filter = new Protocol.TestBeginEndMarkReceiveFilter(begin, end);

            var filterFactory = new DefaultReceiveFilterFactory<TestBeginEndMarkReceiveFilter, StringRequestInfo>();

            AppServer appServer = new AppServer(filterFactory);

            Assert.IsTrue(appServer.Setup("127.0.0.1", 50060));

            Assert.IsTrue(appServer.Start());

            appServer.NewRequestReceived += (s, e) =>
            {
                s.Send("Thank You!");
            };

            EasyClient easyClient = new EasyClient();

            AutoResetEvent callbackEvent = new AutoResetEvent(false);

            easyClient.Initialize(new TestProtoBaseDefaultTerminatorReceiverFilter(), (p) =>
            {
                callbackEvent.Set();
            });

            bool connected = await easyClient.ConnectAsync(serverEndpoint);

            Assert.IsTrue(connected);

            easyClient.Send(Encoding.ASCII.GetBytes("#Welcome!##"));

            callbackEvent.WaitOne(timeout);
        }
    }
}



