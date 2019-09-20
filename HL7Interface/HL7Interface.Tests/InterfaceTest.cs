﻿using System.Net;
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

namespace HL7Interface.Tests
{
    [TestFixture]
    public class InterfaceTest : BaseTests
    {

        [Test, Timeout(timeout)]
        public void TestAsyncTcpSession()
        {
            AppServer appServer = new AppServer();

            Assert.IsTrue(appServer.Setup("127.0.0.1", 50060));

            Assert.IsTrue(appServer.Start());

            appServer.NewRequestReceived += (s, e) =>
            {
                byte[] bytesToSend = Encoding.UTF8.GetBytes("Thank You!" + "||");
                s.Send(bytesToSend, 0, bytesToSend.Length);
            };

            AsyncTcpSession asyncTcpSession = new AsyncTcpSession();

            AutoResetEvent callbackEvent = new AutoResetEvent(false);

            asyncTcpSession.DataReceived += (s, e) =>
            {
                callbackEvent.Set();
            };

            AutoResetEvent connectedEvent = new AutoResetEvent(false);

            asyncTcpSession.Connected += (s, e) =>
            {
                connectedEvent.Set();
            };

            asyncTcpSession.Connect(serverEndpoint);

            Assert.IsTrue(connectedEvent.WaitOne(timeout));


            Assert.IsTrue(asyncTcpSession.IsConnected);

            byte[] data = (Encoding.ASCII.GetBytes("Welcome!" + Environment.NewLine));

            asyncTcpSession.Send(data, 0, data.Length);

            callbackEvent.WaitOne(timeout);

            asyncTcpSession.Close();

            appServer.Stop();
        }


        [Test, Timeout(timeout)]
        public async Task TestEasyClientConnection()
        {
            EasyClient easyClient = new EasyClient();
          
            easyClient.Initialize(new TestProtoBaseFakeTerminatorReceiverFilter(), (p) =>
            {
                //do nothing
            });

            var ret = await easyClient.ConnectAsync(new DnsEndPoint("github.com", 443));

            Assert.True(ret);
        }

        [Test, Timeout(timeout)]
        public async Task TestAppServerEasyClientNewSessionConnected()
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

  

            easyClient.Initialize(new TestProtoBaseFakeTerminatorReceiverFilter(), (p) =>
            {
                //do nothing
            });

            IList<ArraySegment<byte>> buff = null;




            bool connected = await easyClient.ConnectAsync(serverEndpoint);

            Assert.IsTrue(connected);

            var result = easyClient.Socket.BeginReceive(buff, SocketFlags.None, new AsyncCallback((p) =>
            {

            }), null);

            sessionConnectedEvent.WaitOne(timeout);
        }



        public class MyClient : EasyClient
        {
            public MyClient()
            {
                
            }
            protected override void HandlePackage(IPackageInfo package)
            {
                base.HandlePackage(package);
            }
        }



        [Test, Timeout(timeout)]
        public async Task EasyClientSendsWelcomeMesageToAppServerDefaultTerminator()
        {
            AppServer appServer = new AppServer();

            Assert.IsTrue(appServer.Setup("127.0.0.1", 50060));

            Assert.IsTrue(appServer.Start());

            appServer.NewRequestReceived += (s, e) =>
            {
                byte[] bytesToSend = Encoding.ASCII.GetBytes("Thank You!||");
                s.Send(bytesToSend, 0, bytesToSend.Length);
            };

            MyClient easyClient = new MyClient();

            var session = easyClient.GetUnderlyingSession();

            easyClient.ReceiveBufferSize = 30;

            AutoResetEvent callbackEvent = new AutoResetEvent(false);

            easyClient.Initialize(new TestProtoBaseTerminatorReceiverFilter(), (p) =>
            {
                callbackEvent.Set();
            });

            bool connected = await easyClient.ConnectAsync(serverEndpoint);

            Assert.IsTrue(connected);

            easyClient.Send(Encoding.ASCII.GetBytes("Welcome!" + Environment.NewLine));

            callbackEvent.WaitOne(timeout);

            await easyClient.Close();

            appServer.Stop();

        }

        [Test, Timeout(timeout)]
        public async Task TestAppServerendsWelcomeOnEasyClientNewSessionConnected()
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

            easyClient.Initialize(new TestProtoBaseFakeTerminatorReceiverFilter(), (p) =>
            {
                callbackEvent.Set();
            });

            bool connected = await easyClient.ConnectAsync(serverEndpoint);

            Assert.IsTrue(connected);

            callbackEvent.WaitOne(timeout);
        }


        [Test, Timeout(timeout)]
        public async Task TestEasyClientCallBack()
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

            easyClient.Initialize(new TestProtoBaseFakeTerminatorReceiverFilter(), (p) =>
            {
                callbackEvent.Set();
            });

            bool connected = await easyClient.ConnectAsync(serverEndpoint);

            Assert.IsTrue(connected);

            easyClient.Send(Encoding.ASCII.GetBytes("#Welcome!##"));

            callbackEvent.WaitOne(timeout);
        }




        /// <summary>
        /// Start the HL7Interface, initialize it and stop
        /// </summary>
        [Test, Timeout(timeout)]
        public void InterfaceInitializeStartStop()
        {
            HL7InterfaceBase hl7Interface = new HL7InterfaceBase();

            Assert.That(hl7Interface.State == ServerState.NotInitialized);

            Assert.IsTrue(hl7Interface.Initialize());

            Assert.IsTrue(hl7Interface.Start());

            Assert.IsTrue(hl7Interface.State == ServerState.Running);

            hl7Interface.Stop();

            Assert.That(hl7Interface.State == ServerState.NotStarted);
        }

        /// <summary>
        /// Connect an active client to the interface, if a new session is connected  say "Welcome"
        /// </summary>

        [Test, Timeout(timeout)]
        public void ConnectClientToInterface()
        {
            //HL7InterfaceBase hl7Interface = new HL7InterfaceBase();
            //AutoResetEvent newSessionConnectedSignal = new AutoResetEvent(false);
            //AutoResetEvent welcomMessageReceived = new AutoResetEvent(false);

            //Assert.IsTrue(hl7Interface.Initialize());

            //Assert.IsTrue(hl7Interface.Start());

            //hl7Interface.HL7Server.NewSessionConnected += (hl7Session) =>
            //{
            //    Assert.That(hl7Session is HL7Session);
            //    Assert.That(hl7Session.Connected);
            //    hl7Session.Send("#Welcome##");
            //    newSessionConnectedSignal.Set();
            //};
            //EasyClient client = new EasyClient();

            //byte[] begin = Encoding.ASCII.GetBytes("#");
            //byte[] end = Encoding.ASCII.GetBytes("##");
            //client.Initialize(new ReceiverFilter(new HL7ProtocolBase(), begin, end), (packageInfo) =>
            //{
            //    Assert.That(packageInfo.OriginalRequest.Equals("Welcome"));
            //    welcomMessageReceived.Set();
            //});

            //client.ConnectAsync(serverEndpoint).Wait();

            //Assert.That(client.IsConnected);

            //Assert.That(newSessionConnectedSignal.WaitOne());

            //Assert.That(welcomMessageReceived.WaitOne());

            //hl7Interface.Stop();
        }


        /// <summary>
        /// Connect the Interface to the remote endpoint
        /// </summary>

        [Test, Timeout(timeout)]
        public void ConnectInterfaceToServer()
        {
            HL7InterfaceBase hl7Interface = new HL7InterfaceBase();
            HL7Server server = new HL7Server();

            Assert.That(server.Setup("127.0.0.1", 2012));
            Assert.That(server.Start());

            Assert.That(hl7Interface.Initialize());

            Assert.That(hl7Interface.Start());

            Task<bool> connectTask = Task.Run(async ()
               => await hl7Interface.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2012)));

            Assert.That(connectTask.Result);

            server.Stop();

            hl7Interface.Stop();
        }


        /// <summary>
        /// Connect the easy client to the interface and send a new equipment commmand request message
        /// </summary>

        [Test, Timeout(timeout)]
        public void SendMessageToHL7Interface()
        {
            HL7InterfaceBase hl7Interface = new HL7InterfaceBase();
            AutoResetEvent newRequestReceivedConnectedSignal = new AutoResetEvent(false);
            AutoResetEvent acknowledgmentReceivedSignal = new AutoResetEvent(false);

            hl7Interface.Initialize();
            hl7Interface.Start();

            hl7Interface.HL7Server.NewRequestReceived += (hl7Session, hl7Request) =>
            {
                newRequestReceivedConnectedSignal.Set();
            };

            EasyClient client = new EasyClient();

            client.Initialize(new ReceiverFilter(new HL7ProtocolBase()), (packageInfo) =>
            {
                acknowledgmentReceivedSignal.Set();
            });

            client.ConnectAsync(serverEndpoint).Wait();

            byte[] bytesToSend = Encoding.ASCII.GetBytes(MLLP.CreateMLLPMessage((new EquipmentCommandRequest()).Encode()));
            client.Send(bytesToSend);

            Assert.That(newRequestReceivedConnectedSignal.WaitOne());

            Assert.That(acknowledgmentReceivedSignal.WaitOne());

            hl7Interface.Stop();
        }


        /// <summary>
        /// Send the message to the remote system, both the acknowledgment and response are not 
        /// blocking operations.
        /// </summary>
        /// <returns></returns>

        [Test, Timeout(timeout)]
        public async Task SendMessageAsyncTest()
        {
            HL7InterfaceBase hl7Interface = new HL7InterfaceBase();

            HL7Server server = new HL7Server();

            AutoResetEvent requestReceived = new AutoResetEvent(false);

            HL7ProtocolBase protocol = new HL7ProtocolBase(new HL7ProtocolConfig()
            {
                IsAckRequired = false,
                IsResponseRequired = false
            });

            server.Setup("127.0.0.1", 2012);
            server.Start();

            HL7Server serverSide = new HL7Server();
            serverSide.Setup("127.0.0.1", 50060);

            hl7Interface.Initialize(serverSide, protocol);

            hl7Interface.Start();

            server.NewRequestReceived += (e, s) =>
            {
                Assert.That(s.Request is EquipmentCommandRequest);
                requestReceived.Set();
            };

            bool connected = await hl7Interface.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2012));

            Assert.That(connected);

            await hl7Interface.SendHL7MessageAsync(new EquipmentCommandRequest());

            requestReceived.WaitOne();

            server.Stop();

            hl7Interface.Stop();
        }

        /// <summary>
        /// Send the message to the remote system, the acknowledgment  is blocking, and the response  
        /// is not required
        /// </summary>
        /// <returns></returns>

        [Test, Timeout(timeout)]
        public async Task SendMessageAsyncWaitAckTest()
        {
            HL7InterfaceBase hl7Interface = new HL7InterfaceBase();
            HL7Server server = new HL7Server();
            AutoResetEvent requestReceived = new AutoResetEvent(false);

            HL7ProtocolBase protocol = new HL7ProtocolBase(new HL7ProtocolConfig()
            {
                IsAckRequired = true,
                IsResponseRequired = false
            });

            HL7Server serverSide = new HL7Server();
            serverSide.Setup("127.0.0.1", 50060);

            server.Setup("127.0.0.1", 2012);
            server.Start();

            hl7Interface.Initialize(serverSide, protocol);
            hl7Interface.Start();

            server.NewRequestReceived += (e, s) =>
            {
                Assert.That(s.Request is EquipmentCommandRequest);
                requestReceived.Set();
            };

            bool connected = await hl7Interface.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2012));

            Assert.That(connected);

            HL7Request req = await hl7Interface.SendHL7MessageAsync(new EquipmentCommandRequest());

            Assert.IsNotNull(req);

            Assert.That(req.Acknowledgment is GeneralAcknowledgment);

            Assert.That(HL7Parser.IsAckForRequest(req.Request, req.Acknowledgment));

            requestReceived.WaitOne();

            server.Stop();

            hl7Interface.Stop();
        }



        /// <summary>
        /// In this test, the Interface receives an incoming command, executes it and send back 
        /// the response / result  to the client who initiate the transaction.
        /// </summary>
        /// <returns></returns>

        [Test, Timeout(timeout)]
        public async Task SendMessageAsyncWaitAckAndResponseTest()
        {
            HL7InterfaceBase hl7Interface = new HL7InterfaceBase();
            HL7Server server = new HL7Server();
            AutoResetEvent requestReceived = new AutoResetEvent(false);
            HL7ProtocolBase protocol = new HL7ProtocolBase(new HL7ProtocolConfig()
            {
                IsAckRequired = true,
                IsResponseRequired = true
            });

            HL7Server serverSide = new HL7Server();
            serverSide.Setup("127.0.0.1", 50060);

            server.Setup("127.0.0.1", 2012);
            server.Start();

            hl7Interface.Initialize(serverSide, protocol);

            hl7Interface.Start();

            server.NewRequestReceived += (e, s) =>
            {
                Assert.That(s.Request is EquipmentCommandRequest);
                requestReceived.Set();
                Thread.Sleep(500);
                byte[] bytesToSend = Encoding.ASCII.GetBytes(MLLP.CreateMLLPMessage((new EquipmentCommandResponse()).Encode()));
                e.Send(bytesToSend, 0, bytesToSend.Length);
            };

            bool connected = await hl7Interface.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2012));

            Assert.That(connected);

            HL7Request req = await hl7Interface.SendHL7MessageAsync(new EquipmentCommandRequest());

            Assert.IsNotNull(req);

            Assert.That(req.Acknowledgment is GeneralAcknowledgment);

            Assert.That(HL7Parser.IsAckForRequest(req.Request, req.Acknowledgment));

            Assert.That(req.Acknowledgment.GetValue("MSA-1") == "AA");

            Assert.IsNotNull(req.Response);

            Assert.That(req.Response.IsResponseForRequest(req.Request));

            requestReceived.WaitOne();

            server.Stop();

            hl7Interface.Stop();
        }


























        // <summary>
        // Easy client sends message  to HL7Server and receves ack
        // </summary>
        // <returns></returns>

        [Test, Timeout(timeout)]
        public async Task ClientSendsCommandToHl7ServerAndWaitAck()
        {
            AutoResetEvent ackReceived = new AutoResetEvent(false);

            HL7Server hl7Server = new HL7Server();

            hl7Server.Setup("127.0.0.1", 50060);
            hl7Server.Start();

            EquipmentCommandRequest request = new EquipmentCommandRequest();

            EasyClient client = new EasyClient();

            client.Initialize(new ReceiverFilter(new HL7ProtocolBase()), (packageInfo) =>
            {
                if (packageInfo.Request.IsAcknowledge)
                {
                    Assert.That(packageInfo.Request is GeneralAcknowledgment);
                    Assert.That(HL7Parser.IsAckForRequest(request, packageInfo.Request));
                    ackReceived.Set();
                } else
                Assert.Fail();
            });

            await client.ConnectAsync(serverEndpoint);

            Assert.That(client.IsConnected);

            byte[] bytesToSend = Encoding.ASCII.GetBytes(MLLP.CreateMLLPMessage(request.Encode()));
            client.Send(bytesToSend);

            Assert.That(ackReceived.WaitOne());

            hl7Server.Stop();

           await  client.Close();
        }



        // <summary>
        // Easy client sends message  to HL7Server and receves ack
        // </summary>
        // <returns></returns>

        [Test, Timeout(timeout)]
        public async Task ClientSendsCommandToHl7ServerAndWaitAckAndResponse()
        {
            AutoResetEvent ackReceived = new AutoResetEvent(false);

            HL7Server hl7Server = new HL7Server();

            hl7Server.Setup("127.0.0.1", 50060);
            hl7Server.Start();

            EquipmentCommandRequest request = new EquipmentCommandRequest();

            EasyClient client = new EasyClient();

            client.Initialize(new ReceiverFilter(new HL7ProtocolBase()), (packageInfo) =>
            {
                if (packageInfo.Request.IsAcknowledge)
                {
                    Assert.That(packageInfo.Request is GeneralAcknowledgment);
                    Assert.That(HL7Parser.IsAckForRequest(request, packageInfo.Request));
                    ackReceived.Set();
                }
                else
                    Assert.Fail();
            });

            await client.ConnectAsync(serverEndpoint);

            Assert.That(client.IsConnected);

            byte[] bytesToSend = Encoding.ASCII.GetBytes(MLLP.CreateMLLPMessage(request.Encode()));
            client.Send(bytesToSend);

            Assert.That(ackReceived.WaitOne());

            hl7Server.Stop();

            await client.Close();
        }

        /// <summary>
        /// Easy client sends A command to HL7Interface who should acknowledge it
        /// </summary>
        /// <returns></returns>

        [Test, Timeout(timeout)]
        public async Task ClientSendsCommandToHL7InterfaceAndWaitAck()
        {
            HL7InterfaceBase hl7Interface = new HL7InterfaceBase();
            AutoResetEvent ackReceived = new AutoResetEvent(false);
            AutoResetEvent commandResponseReceived = new AutoResetEvent(false);

            HL7ProtocolBase protocol = new HL7ProtocolBase(new HL7ProtocolConfig()
            {
                IsAckRequired = true,
                IsResponseRequired = true
            }) ;

            HL7Server serverSide = new HL7Server();
            Assert.IsTrue(serverSide.Setup("127.0.0.1", 50060));

            Assert.IsTrue(hl7Interface.Initialize(serverSide, protocol));

            Assert.That(hl7Interface.Start());

            EquipmentCommandRequest request = new EquipmentCommandRequest();

            EasyClient client = new EasyClient();

            client.Initialize(new ReceiverFilter(new HL7ProtocolBase()), (packageInfo) =>
            {
                if (packageInfo.Request.IsAcknowledge)
                {
                    Assert.That(packageInfo.Request is GeneralAcknowledgment);
                    Assert.That(HL7Parser.IsAckForRequest(request, packageInfo.Request));
                    ackReceived.Set();
                }
                else
                    Assert.Fail();
            });

            await client.ConnectAsync(serverEndpoint);

            Assert.That(client.IsConnected);

            byte[] bytesToSend = Encoding.ASCII.GetBytes(MLLP.CreateMLLPMessage(request.Encode()));
            client.Send(bytesToSend);

            Assert.That(ackReceived.WaitOne(50000));

            hl7Interface.Stop();

            serverSide.Stop();

            await client.Close();
        }


        // <summary>
        // Send the message to the remote system, both acknowledgment and the response  
        // are blocking operation.
        //
        // Laboratory Device    Automation Manager(HL7INTERFACE)
        //       |                             |
        //       |------------EAC^U07--------->|
        //       |<-----------ACK^U07----------|
        //       |                             |
        //       |<-----------EAR^U07----------|
        //       |------------ACK^U08--------->|
        //       |                             |
        // </summary>
        // <returns></returns>
        [Test, Timeout(timeout)]
        public async Task Hl7InterfaceSendsCommandToAnotherHL7InterfaceWaitAckAndResponse()
        {
            HL7InterfaceBase hl7Interface = new HL7InterfaceBase();
            AutoResetEvent ackReceived = new AutoResetEvent(false);
            AutoResetEvent commandResponseReceived = new AutoResetEvent(false);

            HL7ProtocolBase protocol = new HL7ProtocolBase(new HL7ProtocolConfig()
            {
                IsAckRequired = true,
                IsResponseRequired = true
            });

            HL7Server serverSide = new HL7Server();

            Assert.IsTrue(serverSide.Setup("127.0.0.1", 50060));

            Assert.IsTrue(hl7Interface.Initialize(serverSide, protocol));

            Assert.That(hl7Interface.Start());

            EquipmentCommandRequest request = new EquipmentCommandRequest();

            EasyClient client = new EasyClient();

            client.Initialize(new ReceiverFilter(new HL7ProtocolBase()), (packageInfo) =>
            {
                if (packageInfo.Request.IsAcknowledge)
                {
                    Assert.That(packageInfo.Request is GeneralAcknowledgment);
                    Assert.That(HL7Parser.IsAckForRequest(request, packageInfo.Request));
                    ackReceived.Set();
                }
                else
                    commandResponseReceived.Set();
            });

            await client.ConnectAsync(serverEndpoint);

            Assert.That(client.IsConnected);

            byte[] bytesToSend = Encoding.ASCII.GetBytes(MLLP.CreateMLLPMessage(request.Encode()));

            client.Send(bytesToSend);

            Assert.That(ackReceived.WaitOne());

            Assert.That(commandResponseReceived.WaitOne());

            hl7Interface.Stop();

            await client.Close();

            serverSide.Stop();
        }
    }
}



