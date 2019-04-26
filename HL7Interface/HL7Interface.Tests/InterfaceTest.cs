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
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace HL7Interface.Tests
{
    [TestFixture]
    public class InterfaceTest //: BaseTests
    {
        System.Net.EndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50050);
        System.Net.EndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50060);

        /// <summary>
        /// Start the HL7Interface, initialize it and stop
        /// </summary>
        [Test]
        public void InterfaceInitializeStartStop()
        {
            BaseHL7Interface hl7Interface = new BaseHL7Interface();

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
        [Test]
        public void ConnectClientToInterface()
        {
            BaseHL7Interface hl7Interface = new BaseHL7Interface();
            AutoResetEvent newSessionConnectedSignal = new AutoResetEvent(false);
            AutoResetEvent welcomMessageReceived = new AutoResetEvent(false);

            hl7Interface.Initialize();
            hl7Interface.Start();

            hl7Interface.HL7Server.NewSessionConnected += (hl7Session) =>
            {
                Assert.That(hl7Session is HL7Session);
                Assert.That(hl7Session.Connected);
                hl7Session.Send("#Welcome##");
                newSessionConnectedSignal.Set();
            };
            EasyClient client = new EasyClient();

            byte[] begin = Encoding.UTF8.GetBytes("#");
            byte[] end = Encoding.UTF8.GetBytes("##");
            client.Initialize(new ReceiverFilter(new BaseHL7Protocol(), begin, end), (packageInfo) =>
            {
                Assert.That(packageInfo.OriginalRequest.Equals("Welcome"));
                welcomMessageReceived.Set();
            });

            client.ConnectAsync(serverEndpoint).Wait();

            Assert.That(client.IsConnected);

            Assert.That(newSessionConnectedSignal.WaitOne());

            Assert.That(welcomMessageReceived.WaitOne());

            hl7Interface.Stop();
        }


        /// <summary>
        /// Connect the Interface to the remote endpoint
        /// </summary>
        [Test]
        public void ConnectInterfaceToServer()
        {
            BaseHL7Interface hl7Interface = new BaseHL7Interface();
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
        [Test]
        public void SendMessageToHL7Interface()
        {
            BaseHL7Interface hl7Interface = new BaseHL7Interface();
            AutoResetEvent newRequestReceivedConnectedSignal = new AutoResetEvent(false);
            AutoResetEvent acknowledgmentReceivedSignal = new AutoResetEvent(false);

            hl7Interface.Initialize();
            hl7Interface.Start();

            hl7Interface.HL7Server.NewRequestReceived += (hl7Session, hl7Request) =>
            {
                newRequestReceivedConnectedSignal.Set();
            };

            EasyClient client = new EasyClient();

            client.Initialize(new ReceiverFilter(new BaseHL7Protocol()), (packageInfo) =>
            {
                acknowledgmentReceivedSignal.Set();
            });

            client.ConnectAsync(serverEndpoint).Wait();

            byte[] bytesToSend = Encoding.UTF8.GetBytes(MLLP.CreateMLLPMessage((new EquipmentCommandRequest()).Encode()));
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
        [Test]
        public async Task SendMessageAsyncTest()
        {
            BaseHL7Interface hl7Interface = new BaseHL7Interface();

            HL7Server server = new HL7Server();

            AutoResetEvent requestReceived = new AutoResetEvent(false);

            BaseHL7Protocol protocol = new BaseHL7Protocol(new HL7ProtocolConfig()
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
        [Test]
        public async Task SendMessageAsyncWaitAckTest()
        {
            BaseHL7Interface hl7Interface = new BaseHL7Interface();
            HL7Server server = new HL7Server();
            AutoResetEvent requestReceived = new AutoResetEvent(false);

            BaseHL7Protocol protocol = new BaseHL7Protocol(new HL7ProtocolConfig()
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
        [Test]
        public async Task SendMessageAsyncWaitAckAndResponseTest()
        {
            BaseHL7Interface hl7Interface = new BaseHL7Interface();
            HL7Server server = new HL7Server();
            AutoResetEvent requestReceived = new AutoResetEvent(false);
            BaseHL7Protocol protocol = new BaseHL7Protocol(new HL7ProtocolConfig()
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

                //EasyClient client = new EasyClient();
                //client.Initialize(new ReceiverFilter(new BaseHL7Protocol()), (packageInfo) =>
                //{
                //    Assert.That(packageInfo.Request is GeneralAcknowledgment);
                //});

                //client.ConnectAsync(serverEndpoint).Wait();

                byte[] bytesToSend = Encoding.UTF8.GetBytes(MLLP.CreateMLLPMessage((new EquipmentCommandResponse()).Encode()));
                //client.Send(bytesToSend);
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
        [Test]//, Repeat(2)]
        [Timeout(30000)]
        public async Task SendSollicitedCommandToInterface()
        {
            BaseHL7Interface hl7Interface = new BaseHL7Interface();
            AutoResetEvent ackReceived = new AutoResetEvent(false);
            AutoResetEvent commandResponseReceived = new AutoResetEvent(false);
            BaseHL7Protocol protocol = new BaseHL7Protocol(new HL7ProtocolConfig()
            {
                IsAckRequired = true,
                IsResponseRequired = true
            });

            HL7Server serverSide = new HL7Server();
            serverSide.Setup("127.0.0.1", 50060);
            hl7Interface.Initialize(serverSide, protocol);

            hl7Interface.Start();

            EquipmentCommandRequest request = new EquipmentCommandRequest();
            EasyClient client = new EasyClient();
            client.Initialize(new ReceiverFilter(new BaseHL7Protocol()), (packageInfo) =>
            {
                if(packageInfo.Request.IsAcknowledge)
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

            byte[] bytesToSend = Encoding.UTF8.GetBytes(MLLP.CreateMLLPMessage(request.Encode()));
            client.Send(bytesToSend);

            Assert.That(ackReceived.WaitOne());

            Assert.That(commandResponseReceived.WaitOne());

            hl7Interface.Stop();
        }
    }
}



