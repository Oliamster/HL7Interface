using System;
using System.Diagnostics;
using HL7api.Parser;
using NHapi.Model.V251.Message;
using HL7api.V251.Message;
using NUnit.Framework;

namespace HL7api.Tests
{
    [TestFixture]
    public class HL7ParserTests
    {
        [Test]
        public void CreateNewAdmitVisitNotification()
        {
            AdmitVisitNotification avn = new AdmitVisitNotification();

            Assert.IsAssignableFrom(typeof(ADT_A01), avn.ADT_A01);

            Assert.That(avn.MessageDateTime.ToString("yyyyMMddHHmmss"), Does.Match(""));

            Assert.AreEqual("ADT", avn.MessageCode);

            Assert.AreEqual("A01", avn.TriggerEvent);

            Guid g = new Guid();
            Assert.IsTrue(Guid.TryParse(avn.ControlID, out g));

            Assert.IsFalse(g.ToString() == "00000000-0000-0000-0000-000000000000");

            string msg = avn.Encode();

            Debug.Print(msg);

        }


        [Test]
        public void TestPositiveAckConstruction()
        {
            PrepareForSpecimenRequest prepare = new PrepareForSpecimenRequest();

            HL7Parser p = new HL7Parser();

            ParserResult result = p.Parse(prepare.Encode());

            Assert.IsTrue(result.MessageAccepted);

            Assert.IsTrue(result.Acknowledge.IsAckForRequest(prepare));           
        }

        [Test]
        public void TestPositiveAckPrepareForSpecimenResponse()
        {
            PrepareForSpecimenResponse prepare = new PrepareForSpecimenResponse();

            HL7Parser p = new HL7Parser();

            ParserResult result = p.Parse(prepare.Encode());

            Assert.IsTrue(result.MessageAccepted);

            Assert.IsTrue(result.Acknowledge.IsAckForRequest(prepare));
        }
    }
}

