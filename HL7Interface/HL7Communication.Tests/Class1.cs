using AutomationManager;
using HL7DeviceSimulator;
using NUnit.Framework;
using System.Net;

namespace HL7Communication.Tests
{
    [TestFixture]
    public class ComunicationTestBase
    {
        protected IPEndPoint m_DeviceIPEndpoint;
        protected Device m_AutomatedDevice;
        protected DeviceManager m_AutomationManager;

        [SetUp]
        public void InitTest()
        {

            m_DeviceIPEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2012);
            m_AutomatedDevice = new Device();
            m_AutomationManager = new DeviceManager();

            m_AutomatedDevice.Init();
        }

        [Test]
        public void ComunicationTest1()
        {
        }
    }
}
