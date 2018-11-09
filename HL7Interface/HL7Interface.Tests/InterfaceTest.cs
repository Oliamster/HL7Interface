using HL7api.V251.Message;
using Hl7Interface.ServerProtocol;
using HL7Interface.ClientProtocol;
using HL7Interface.ServerProtocol;
using NUnit.Framework;
using SuperSocket.ClientEngine;
using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HL7Interface.Tests
{
    [TestFixture]
    public class InterfaceTest
    {

        System.Net.EndPoint clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50050);
        System.Net.EndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50060);


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
                Assert.That(packageInfo.OriginalRequest.Equals("#Welcome##"));
                welcomMessageReceived.Set();
            });

            client.ConnectAsync(serverEndpoint).Wait();

            Assert.That(client.IsConnected);

            Assert.That(newSessionConnectedSignal.WaitOne());

            Assert.That(welcomMessageReceived.WaitOne());

            hl7Interface.Stop();
        }


        [Test]
        public void ConnectInterfaceToServer()
        {
            BaseHL7Interface hl7Interface = new BaseHL7Interface();
            HL7Server server = new HL7Server();

            Assert.That(server.Setup("127.0.0.1", 2012));
            Assert.That(server.Start());

            Assert.That(hl7Interface.Initialize());
            Assert.That(hl7Interface.Start());

            Task<bool> connectTask = Task.Run( async () 
                => await hl7Interface.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2012)));

            Assert.That(connectTask.Result);

            server.Stop();

            hl7Interface.Stop();
        }

        [Test]
        public async Task SendMessageAsyncTest()
        {
            BaseHL7Interface hl7Interface = new BaseHL7Interface();
            HL7Server server = new HL7Server();
            AutoResetEvent requestReceived = new AutoResetEvent(false);

            server.Setup("127.0.0.1", 2012);
            server.Start();

            hl7Interface.Initialize();
            hl7Interface.Start();

            server.NewRequestReceived += (e, s) =>
            {
                Assert.That(s.RequestMessage is EquipmentCommandRequest);
                requestReceived.Set();
            };

            bool connected = await hl7Interface.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2012));

            Assert.That(connected);

             hl7Interface.SendHL7MessageAsync(new EquipmentCommandRequest()).Wait();

            requestReceived.WaitOne();

            server.Stop();

            hl7Interface.Stop();
        }
    }
}


