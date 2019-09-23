using NUnit.Framework;
using SuperSocket.ClientEngine;
using SuperSocket.SocketBase;
using System;
using System.Net;
using System.Reflection;

namespace HL7Interface.Tests.BasicTest
{
    public class BasicTestsBase
    {
        protected const int timeout = 50000000;
        protected System.Net.EndPoint clientEndPoint;
        protected System.Net.EndPoint serverEndpoint;
        protected EasyClient easyClient;
        protected AppServer appServer;
       

        
        [SetUp]
        public void TestInitialize()
        {
            clientEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50050);
            serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 50060);
            //easyClient = new EasyClient();
            //appServer = new AppServer();

            //Assert.IsTrue(appServer.Setup("127.0.0.1", 50060));

            //Assert.IsTrue(appServer.Start());
        }

        [TearDown]
        public void TestTearDown()
        {
            //easyClient.Close().Wait();

            //appServer.Stop();
        }

 

        public BasicTestsBase()
        {
            /* Preparing test start */
            Assembly assembly = Assembly.GetCallingAssembly();
            AppDomainManager manager = new AppDomainManager();
            FieldInfo entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            entryAssemblyfield.SetValue(manager, assembly);

            AppDomain domain = AppDomain.CurrentDomain;
            FieldInfo domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField.SetValue(domain, manager);
            /* Preparing test end */
        }
    }
}