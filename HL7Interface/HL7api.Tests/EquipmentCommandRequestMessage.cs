
using NHapi.Model.V251.Message;
using HL7api.V251.Message;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HL7api.Tests
{
    [TestFixture]
    public class EquipmentCommandRequestMessage
    {
        [Test]
        public void CreateNewEquipmentCommandRequest()
        {
            EquipmentCommandRequest request = new EquipmentCommandRequest();

            Assert.IsAssignableFrom(typeof(EAC_U07), request.EAC_U07);

            Assert.That(request.MessageDateTime.ToString("yyyyMMddHHmmss"), Does.Match(""));

            Assert.AreEqual("EAC", request.Code);

            Assert.AreEqual("U07", request.Trigger);

            Guid g = new Guid();
            Assert.IsTrue(Guid.TryParse(request.MessageID, out g));

            Assert.IsFalse(g.ToString() == "00000000-0000-0000-0000-000000000000");

            string msg = request.Encode();

            Debug.Print(msg);

        }
    }
}

