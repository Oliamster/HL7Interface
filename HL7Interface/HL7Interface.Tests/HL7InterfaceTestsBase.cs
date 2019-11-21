using System;
using System.Net;
using System.Reflection;
using HL7Interface.Configuration;
using NUnit.Framework;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;

namespace HL7Interface.Tests
{
    public class HL7InterfaceTestsBase
    {
        protected const int timeout = 10000;
        protected System.Net.EndPoint endPointB;
        protected System.Net.EndPoint endPointA;
        protected HL7InterfaceBase hl7InterfaceA;
        protected HL7InterfaceBase hl7InterfaceB;
       
        [SetUp]
        public void TestInitialize()
        {
            endPointB = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50050);
            endPointA = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50060);
        }


        protected void CreateAndConfigureA(
            bool ackRequired = true, 
            bool responseRequired = false,
            int connectionTimeout = -1,
            int ackTimeout = HL7ProtocolConfig.DefaultAcktimeout,
            int responseTimeout = HL7ProtocolConfig.DefaultResponseTimeout)
        {
            hl7InterfaceA = new HL7InterfaceBase();

            HL7ProtocolConfig hL7ProtocolConfigA = new HL7ProtocolConfig()
            {
                IsAckRequired = ackRequired,
                IsResponseRequired = responseRequired,
                ConnectionTimeout = connectionTimeout,
                AckTimeout = ackTimeout
            };

            IServerConfig serverConfigA = new ServerConfig()
            {
                Ip = "127.0.0.1",
                Port = 50060
            };

            hl7InterfaceA.Initialize(hL7ProtocolConfigA, serverConfigA);
        }


        protected void CreateAndConfigureB(bool ackRequired = true,
            bool responseRequired = false,
            int connectionTimeout = -1,
            int ackTimeout = HL7ProtocolConfig.DefaultAcktimeout,
            int responseTimeout = HL7ProtocolConfig.DefaultResponseTimeout)
        { 
            hl7InterfaceB = new HL7InterfaceBase();

            HL7ProtocolConfig hL7ProtocolConfigB = new HL7ProtocolConfig()
            {
                IsAckRequired = ackRequired,
                AckTimeout = ackTimeout,
                IsResponseRequired = responseRequired,
                ConnectionTimeout = connectionTimeout,
            };

            IServerConfig serverConfigB = new ServerConfig()
            {
                Ip = "127.0.0.1",
                Port = 50050
            };

            hl7InterfaceB.Initialize(hL7ProtocolConfigB, serverConfigB);
        }


        [TearDown]
        public void TestTearDown()
        {

        }

        protected void CreateClient()
        {

        }
        public HL7InterfaceTestsBase()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            AppDomainManager manager = new AppDomainManager();
            FieldInfo entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            entryAssemblyfield.SetValue(manager, assembly);

            AppDomain domain = AppDomain.CurrentDomain;
            FieldInfo domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField.SetValue(domain, manager);
        }
    }
}