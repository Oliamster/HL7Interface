
using NHapi.Model.V251.Message;
using HL7api.V251.Message;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HL7api.Parser;
using HL7api.Model;

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

        [Test]
        public void ParseEquipmentCommandRequest()
        {
            HL7Parser p = new HL7Parser(); 
            string m = @"MSH|^~\&|||||20181004003016||EAC^U07|d241a178-b714-49ce-9177-52a572e2f419||2.5.1|||||||||EquipmentCommandRequestECD||CL^Prepare for specimen acquisition^ASTM|Y";

            ParserResult message = p.Parse(m);
        }
    }
}

