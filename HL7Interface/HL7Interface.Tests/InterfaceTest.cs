using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Interface.Tests
{
    [TestFixture]
    public class InterfaceTest
    {
        [Test]
        public void StartServer()
        {
            BaseHL7Interface hl7Interface = new BaseHL7Interface();
            Assert.IsTrue(hl7Interface.Initialise());
        }
    }
}
