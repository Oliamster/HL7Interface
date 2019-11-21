using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HL7api.Model;
using HL7api.Parser;
using HL7api.V251.Message;
using Hl7Interface.ServerProtocol;
using HL7Interface.ClientProtocol;
using HL7Interface.Configuration;
using HL7Interface.ServerProtocol;
using HL7Interface.Tests.Protobase;
using NHapiTools.Base.Util;
using NUnit.Framework;
using SuperSocket.ClientEngine;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;

namespace HL7Interface.Tests
{
    [TestFixture]
    public class HL7InterfaceTests : HL7InterfaceTestsBase
    {
        /// <summary>
        /// Start the HL7Interface, initialize it and stop
        /// </summary>
        [Test, Timeout(timeout)]
        public void A_InterfaceInitializeStartStop()
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
        /// Connect the HL7Interface to the AppServer
        /// </summary>
        [Test, Timeout(timeout)]
        public async Task B_ConnectHL7InterfaceToAppServer()
        {
            HL7InterfaceBase hl7Interface = new HL7InterfaceBase();

            HL7Server server = new HL7Server();

            Assert.That(server.Setup("127.0.0.1", 2012));
            Assert.That(server.Start());

            Assert.That(hl7Interface.Initialize());

            Assert.That(hl7Interface.Start());

            bool connected = await hl7Interface.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2012));

            Assert.That(connected);

            server.Stop();

            hl7Interface.Stop();
        }

        /// <summary>
        /// Connect an active client to the interface, if a new session is connected  say "Welcome"
        /// </summary>

        [Test, Timeout(timeout)]
        public async Task C_ConnectEasyClientToHL7Interface()
        {
            HL7InterfaceBase hl7Interface = new HL7InterfaceBase();
            AutoResetEvent newSessionConnectedSignal = new AutoResetEvent(false);
            AutoResetEvent welcomMessageReceived = new AutoResetEvent(false);

            Assert.IsTrue(hl7Interface.Initialize());

            Assert.IsTrue(hl7Interface.Start());

            hl7Interface.HL7Server.NewSessionConnected += (hl7Session) =>
            {
                Assert.That(hl7Session is HL7Session);
                Assert.That(hl7Session.Connected);
                newSessionConnectedSignal.Set();
            };

            EasyClient client = new EasyClient();

            client.Initialize(new TestProtoBaseDefaultTerminatorReceiverFilter(), (packageInfo) =>
            {
                //nothing
            });

            Assert.IsTrue(client.ConnectAsync(endPointA).Result);

            Assert.That(client.IsConnected);

            Assert.That(newSessionConnectedSignal.WaitOne());

            await client.Close();

            hl7Interface.Stop();
        }

        /// <summary>
        /// Connect an active client to the interface, if a new session is connected  say "Welcome"
        /// </summary>

        [Test, Timeout(timeout)]
        public void D_HL7InterfaceSendsWelcomeOnEasyClientNewSessionConnected()
        {
            HL7InterfaceBase hl7Interface = new HL7InterfaceBase();

            AutoResetEvent welcomMessageReceived = new AutoResetEvent(false);

            Assert.IsTrue(hl7Interface.Initialize());

            Assert.IsTrue(hl7Interface.Start());

            hl7Interface.HL7Server.NewSessionConnected += (hl7Session) =>
            {
                Assert.That(hl7Session is HL7Session);

                Assert.That(hl7Session.Connected);

                byte[] data = Encoding.ASCII.GetBytes("|Welcome!||");

                hl7Session.Send(data, 0, data.Length);
            };

            EasyClient client = new EasyClient();

            client.Initialize(new TestProtoBaseBeginEndMarkReceiverFilter(), (packageInfo) =>
            {
                Assert.That(packageInfo.OriginalRequest.Equals("Welcome!"));
                welcomMessageReceived.Set();
            });

            Assert.IsTrue(client.ConnectAsync(endPointA).Result);

            Assert.That(client.IsConnected);

            Assert.That(welcomMessageReceived.WaitOne());

            hl7Interface.Stop();
        }


        [Test, Timeout(timeout)]
        public async Task E_EasyClientSendsHL7MessageToHL7InterfaceAndReceivesAck()
        {
            HL7InterfaceBase hl7Interface = new HL7InterfaceBase();

            AutoResetEvent newRequestReceived = new AutoResetEvent(false);

            Assert.IsTrue(hl7Interface.Initialize());

            Assert.IsTrue(hl7Interface.Start());

            PrepareForSpecimenRequest equipmentCommandRequest = new PrepareForSpecimenRequest();

            hl7Interface.HL7Server.NewRequestReceived += (hl7Session, hl7Request) =>
            {
                Assert.That(hl7Request is HL7Request);

                Assert.IsTrue(hl7Request.Request is PrepareForSpecimenRequest);

                Assert.That(hl7Session.Connected);
            };

            EasyClient client = new EasyClient();

            var tcs = new TaskCompletionSource<IHL7Message>();

            client.Initialize(new ReceiverFilter(new HL7ProtocolBase()), (packageInfo) =>
            {
                tcs.SetResult(packageInfo.Request);
            });

            Assert.That(await client.ConnectAsync(endPointA));

            byte[] data = Encoding.ASCII.GetBytes(MLLP.CreateMLLPMessage(equipmentCommandRequest.Encode()));

            client.Send(data);

            var result = await tcs.Task;

            Assert.IsTrue(result.IsAcknowledge);

            Assert.IsTrue(HL7Parser.IsAckForRequest(equipmentCommandRequest, result));

            await client.Close();

            hl7Interface.Stop();
        }

        [Test, Timeout(timeout)]
        public async Task F_HL7InterfaceReceivesHL7MessageSendsAResponseToEasyClient()
        {
            HL7InterfaceBase hl7Interface = new HL7InterfaceBase();

            AutoResetEvent newRequestReceived = new AutoResetEvent(false);

            Assert.IsTrue(hl7Interface.Initialize());

            Assert.IsTrue(hl7Interface.Start());

            PrepareForSpecimenRequest equipmentCommandRequest = new PrepareForSpecimenRequest();

            hl7Interface.HL7Server.NewSessionConnected += (hl7Session) =>
            {
                Assert.That(hl7Session.Connected);

                string response = MLLP.CreateMLLPMessage((new PrepareForSpecimenResponse()).Encode());

                byte[] dataToSend = Encoding.ASCII.GetBytes(response);

                hl7Session.Send(dataToSend, 0, dataToSend.Length);
            };

            EasyClient client = new EasyClient();

            var tcs = new TaskCompletionSource<IHL7Message>();

            client.Initialize(new ReceiverFilter(new HL7ProtocolBase()), (packageInfo) =>
            {
                tcs.SetResult(packageInfo.Request);
            });

            Assert.That(await client.ConnectAsync(endPointA));

            var result = await tcs.Task;

            Assert.IsTrue(result is PrepareForSpecimenResponse);

            await client.Close();

            hl7Interface.Stop();
        }

        /// <summary>
        /// Send the message to the remote system, both the acknowledgment and response are not 
        /// blocking operations.
        /// </summary>
        /// <returns></returns>

        [Test, Timeout(timeout)]

        public async Task G_HL7InterfaceSendsMessageToHL7ServerNoAckNoResponse()
        {
            HL7InterfaceBase hl7Interface = new HL7InterfaceBase();

            HL7Server server = new HL7Server();

            AutoResetEvent requestReceived = new AutoResetEvent(false);

            HL7ProtocolBase protocol = new HL7ProtocolBase(new HL7ProtocolConfig()
            {
                IsAckRequired = false,
                IsResponseRequired = false
            });

            server.Setup("127.0.0.1", 50050);
            server.Start();

            HL7Server serverSide = new HL7Server();
            serverSide.Setup("127.0.0.1", 50060);

            hl7Interface.Initialize(serverSide, protocol);

            hl7Interface.Start();

            server.NewRequestReceived += (e, s) =>
            {
                Assert.That(s.Request is PrepareForSpecimenRequest);
                requestReceived.Set();
            };

            bool connected = await hl7Interface.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50050));

            Assert.That(connected);

            await hl7Interface.SendHL7MessageAsync(new PrepareForSpecimenRequest());

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
        public async Task H_HL7InterfaceSendsMessageToHL7ServerWaitAck()
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
                Assert.That(s.Request is PrepareForSpecimenRequest);
                requestReceived.Set();
            };

            bool connected = await hl7Interface.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2012));

            Assert.That(connected);

            HL7Request req = await hl7Interface.SendHL7MessageAsync(new PrepareForSpecimenRequest());

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
        public async Task I_HL7InterfaceSendsMessageToHL7ServerWaitAckAndResponseTest()
        {
            HL7InterfaceBase hl7Interface = new HL7InterfaceBase();
            HL7Server server = new HL7Server();
            AutoResetEvent requestReceived = new AutoResetEvent(false);

            HL7ProtocolBase protocol = new HL7ProtocolBase(new HL7ProtocolConfig()
            {
                IsAckRequired = true,
                IsResponseRequired = true,
                AckTimeout = 10000,
                ResponseTimeout = 50000
            });

            HL7Server serverSide = new HL7Server();
            serverSide.Setup("127.0.0.1", 50060);

            server.Setup("127.0.0.1", 2012);
            server.Start();

            hl7Interface.Initialize(serverSide, protocol);

            hl7Interface.Start();

            server.NewRequestReceived += (e, s) =>
            {
                Assert.That(s.Request is PrepareForSpecimenRequest);
                requestReceived.Set();
                Thread.Sleep(500);
                byte[] bytesToSend = Encoding.ASCII.GetBytes(MLLP.CreateMLLPMessage((new PrepareForSpecimenResponse()).Encode()));
                e.Send(bytesToSend, 0, bytesToSend.Length);
            };

            bool connected = await hl7Interface.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2012));

            Assert.That(connected);

            HL7Request req = await hl7Interface.SendHL7MessageAsync(new PrepareForSpecimenRequest());

            Assert.IsNotNull(req);

            Assert.That(req.Acknowledgment is GeneralAcknowledgment);

            Assert.That(req.Acknowledgment.IsAckForRequest(req.Request));

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
        public async Task J_EasyClientSendsCommandToHL7ServerAndWaitAck()
        {
            AutoResetEvent ackReceived = new AutoResetEvent(false);

            HL7Server hl7Server = new HL7Server();

            hl7Server.Setup("127.0.0.1", 50060);

            hl7Server.Start();

            PrepareForSpecimenRequest request = new PrepareForSpecimenRequest();

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

            Assert.That(client.ConnectAsync(endPointA).Result);

            Assert.That(client.IsConnected);

            byte[] bytesToSend = Encoding.ASCII.GetBytes(MLLP.CreateMLLPMessage(request.Encode()));
            client.Send(bytesToSend);

            Assert.That(ackReceived.WaitOne());

            await client.Close();

            hl7Server.Stop();
        }

        // <summary>
        // Easy client sends message  to HL7Server and receves ack
        // </summary>
        // <returns></returns>

        [Test, Timeout(timeout)]
        public async Task K_EasyClientSendsCommandToHL7ServerAndWaitAck()
        {
            AutoResetEvent ackReceived = new AutoResetEvent(false);

            HL7Server hl7Server = new HL7Server();

            hl7Server.Setup("127.0.0.1", 50060);
            hl7Server.Start();

            PrepareForSpecimenRequest request = new PrepareForSpecimenRequest();

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

            Assert.That(client.ConnectAsync(endPointA).Result);

            Assert.That(client.IsConnected);

            byte[] bytesToSend = Encoding.ASCII.GetBytes(MLLP.CreateMLLPMessage(request.Encode()));
            client.Send(bytesToSend);

            Assert.That(ackReceived.WaitOne());

            await client.Close();

            hl7Server.Stop();

        }

        /// <summary>
        /// Easy client sends A command to HL7Interface who should acknowledge it
        /// </summary>
        /// <returns></returns>

        [Test, Timeout(timeout)]
        public async Task L_EasyClientSendsCommandToHL7InterfaceAndWaitAck()
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

            PrepareForSpecimenRequest request = new PrepareForSpecimenRequest();

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

            Assert.That(client.ConnectAsync(endPointA).Result);

            byte[] bytesToSend = Encoding.ASCII.GetBytes(MLLP.CreateMLLPMessage(request.Encode()));
            client.Send(bytesToSend);

            Assert.That(ackReceived.WaitOne(timeout));

            await client.Close();

            hl7Interface.Stop();
        }




        // Automation Manager         Laboratory Device          
        // (HL7INTERFACE)              (HL7INTERFACE)     
        //       |----------EAC^U07--------->|
        //       |<---------ACK^U07----------|
        //       |                           |
        //       |<---------EAR^U07----------|
        //       |----------ACK^U08--------->|
        //       |                           |
        // </summary>
        // <returns></returns>
        [Test, Timeout(timeout)]
        public async Task M_EasyCLientSendsCommandToHL7InterfaceWaitAckAndResponse()
        {
            HL7InterfaceBase hl7Interface = new HL7InterfaceBase();
            AutoResetEvent ackReceived = new AutoResetEvent(false);
            AutoResetEvent commandResponseReceived = new AutoResetEvent(false);

            HL7Server serverSide = new HL7Server();

            Assert.IsTrue(serverSide.Setup("127.0.0.1", 50060));

            Assert.IsTrue(hl7Interface.Initialize(serverSide, new HL7ProtocolBase( new HL7ProtocolConfig()) ));

            Assert.That(hl7Interface.Start());

            hl7Interface.NewRequestReceived += (s, e) =>
            {
                string response = MLLP.CreateMLLPMessage((new PrepareForSpecimenResponse()).Encode());

                byte[] dataToSend = Encoding.ASCII.GetBytes(response);

                s.Send(dataToSend, 0, dataToSend.Length);
            };

            PrepareForSpecimenRequest request = new PrepareForSpecimenRequest();

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
                {
                    Assert.IsTrue(packageInfo.Request is PrepareForSpecimenResponse);
                    commandResponseReceived.Set();
                }
            });

            Assert.That(client.ConnectAsync(endPointA).Result);

            byte[] bytesToSend = Encoding.ASCII.GetBytes(MLLP.CreateMLLPMessage(request.Encode()));

            client.Send(bytesToSend);

            Assert.That(ackReceived.WaitOne());

            Assert.That(commandResponseReceived.WaitOne());

            await client.Close();

            serverSide.Stop();

            hl7Interface.Stop();
        }


        // Automation Manager         Laboratory Device          
        // (HL7INTERFACE)              (HL7INTERFACE)     
        //       |----------EAC^U07--------->|
        //       |<---------ACK^U07----------|
        //       |                           |
        //       |<---------EAR^U07----------|
        //       |----------ACK^U08--------->|
        //       |                           |
        // </summary>
        // <returns></returns>
        [Test, Timeout(timeout + 5000), Repeat(3)]
        public async Task N_HL7InterfaceSendsCommandToHL7InterfaceWaitAckAndResponse()
        {
            CreateAndConfigureA(true, false);
            CreateAndConfigureB(true, true);

            AutoResetEvent ackReceived = new AutoResetEvent(false);

            AutoResetEvent commandResponseReceived = new AutoResetEvent(false);
  
            Assert.That(hl7InterfaceA.Start());

            hl7InterfaceA.NewRequestReceived += async (s, e) =>
            {
                PrepareForSpecimenResponse rsp = new PrepareForSpecimenResponse()
                {
                    RequestID = e.Request.MessageID
                };

                Assert.That(await hl7InterfaceA.ConnectAsync(endPointB));

                await hl7InterfaceA.SendHL7MessageAsync(rsp);
            };

            Assert.That(hl7InterfaceB.Start());

            PrepareForSpecimenRequest request = new PrepareForSpecimenRequest();

            Assert.That(await hl7InterfaceB.ConnectAsync(endPointA));

            var result = await hl7InterfaceB.SendHL7MessageAsync(request);

            Assert.IsTrue(result.Acknowledgment != null);

            Assert.IsTrue(HL7Parser.IsAckForRequest(request, result.Acknowledgment));

            Assert.IsTrue(result.Response.IsResponseForRequest(request));

            hl7InterfaceA.Stop(); hl7InterfaceB.Stop();
        }

        [Test]
        public async Task O_HL7InterfaceSelfConnectShouldFail()
        {
            HL7InterfaceBase hl7InterfaceA = new HL7InterfaceBase();

            HL7ProtocolBase protocolA = new HL7ProtocolBase(new HL7ProtocolConfig()
            {
                IsAckRequired = true,
                IsResponseRequired = false
            });

            HL7Server serverA = new HL7Server();

            Assert.IsTrue(serverA.Setup("127.0.0.1", 50060));

            Assert.IsTrue(hl7InterfaceA.Initialize(serverA, protocolA));

            Assert.That(hl7InterfaceA.Start());

            hl7InterfaceA.NewSessionConnected += (e) =>
            {

            };

            Assert.IsFalse(await hl7InterfaceA.ConnectAsync(endPointA));

            hl7InterfaceA.Stop();
        }

        [Test, Timeout(timeout)]
        public async Task P_Stopping_Interface_Should_Cancel_Incompleted_SenderTask()
        {
            CreateAndConfigureA(true, true, 5000);
            CreateAndConfigureB(true, false);

            AutoResetEvent ackReceived = new AutoResetEvent(false);

            AutoResetEvent commandResponseReceived = new AutoResetEvent(false);

            Assert.That(hl7InterfaceA.Start());

            Assert.That(hl7InterfaceB.Start());

            PrepareForSpecimenRequest request = new PrepareForSpecimenRequest();

            Assert.That(await hl7InterfaceA.ConnectAsync(endPointB));

            var senderTask =  hl7InterfaceA.SendHL7MessageAsync(request);

            await Task.Delay(10);

            hl7InterfaceA.Stop();

            Assert.IsTrue(senderTask.IsCompleted);

            hl7InterfaceB.Stop();
        }


        
        [Test, Timeout(timeout + 1000000000)]
        public void  R_Ack_Not_Received_Scenario()
        {
            CreateAndConfigureA(true, false, 5000);

            FakeHL7Server fakeHL7Server = new FakeHL7Server();

            IServerConfig fakeHL7ServerConfig = new ServerConfig()
            {
                Ip = "127.0.0.1",
                Port = 50050
            };

            Assert.That(hl7InterfaceA.Start());

            Assert.IsTrue(fakeHL7Server.Setup(fakeHL7ServerConfig));

            Assert.That(fakeHL7Server.Start());

            var connectionTask = hl7InterfaceA.ConnectAsync(endPointB);

            PrepareForSpecimenRequest request = new PrepareForSpecimenRequest();

            Assert.ThrowsAsync(Is.TypeOf<HL7InterfaceException>()

                .And.Message.EqualTo("The message was not acknowledged after a total number of -1 retries"),  

                async () =>  await  hl7InterfaceA.SendHL7MessageAsync(request));

            fakeHL7Server.Stop(); hl7InterfaceA.Stop();
        }

        [Test, Timeout(timeout + 5000), Repeat(2)]
        public async Task Q_Stopping_Interface_Should_Cancel_Connection_Routine()
        {
            CreateAndConfigureA(true, false, -1);
            CreateAndConfigureB(true, false);

            Assert.That(hl7InterfaceA.Start());

            //Assert.That(hl7InterfaceB.Start());

            var connectionTask = hl7InterfaceA.ConnectAsync(endPointB);

            await Task.Delay(10);

            Assert.IsFalse(hl7InterfaceA.IsConnected);

            hl7InterfaceA.Stop();

            Assert.ThrowsAsync(Is.TypeOf<OperationCanceledException>()

                 .And.Message.EqualTo("The connection task was cancelled."),

                 async () => await connectionTask);

            Assert.IsTrue(connectionTask.IsCompleted);


            Assert.IsFalse(hl7InterfaceA.IsConnected);

            hl7InterfaceB.Stop();
        }


        [Test, Timeout(timeout)]
        public async Task S_Throw_If_Connection_timed_out()
        {
            CreateAndConfigureA(true, false, 100);

            CreateAndConfigureB(true, false);

            Assert.That(hl7InterfaceA.Start());

            //Assert.That(hl7InterfaceB.Start());

            var connectionTask = hl7InterfaceA.ConnectAsync(endPointB);

            await Task.Delay(10);

            Assert.IsFalse(hl7InterfaceA.IsConnected);

            Assert.ThrowsAsync(Is.TypeOf<HL7InterfaceException>()

               .And.Message.EqualTo("Connection timed out: 100ms."),

               async () => await connectionTask);

            Assert.IsFalse(hl7InterfaceA.IsConnected);

            hl7InterfaceB.Stop(); hl7InterfaceA.Stop();
        }


        [Test, Timeout(-1)]
        public void T_Handle_And_Acknowledge_messages_Concurrently()
        {
            CreateAndConfigureA(true, false, 5000, -1);

            CreateAndConfigureB(true, false, 5000, -1);

            Assert.That(hl7InterfaceA.Start());

            Assert.That(hl7InterfaceB.Start());

            var connectionTask = hl7InterfaceA.ConnectAsync(endPointB);

            Assert.IsTrue(connectionTask.Result);

            Assert.IsTrue(hl7InterfaceA.IsConnected);

            List<Task> tasks = new List<Task>(100);

            for (int i = 0; i < 1; i++)
            {
                tasks.Add(hl7InterfaceA.SendHL7MessageAsync(new PrepareForSpecimenRequest($"{(100 + i).ToString()}")));
            }
            Task.WaitAll(tasks.ToArray());

            hl7InterfaceB.Stop(); hl7InterfaceA.Stop();
        }
    }
}