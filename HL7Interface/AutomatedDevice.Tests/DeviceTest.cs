using HL7DeviceSimulator;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomatedDevice.Tests
{
    public class DeviceTest
    {
        [Test]
        public void DeviceInitialization()
        {
            Device d = new Device();
            d.Init();
        }
    }
}
