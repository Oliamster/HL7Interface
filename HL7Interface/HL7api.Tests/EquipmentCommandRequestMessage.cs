
using NHapi.Model.V251.Message;
using HL7api.V251.Message;
using NUnit.Framework;
using System;
using System.Diagnostics;
using HL7api.Parser;

namespace HL7api.Tests
{
    [TestFixture]
    public class PrepareForSpecimenResponseMessageTest
    {
        [Test]
        public void CreatePrepareForSpecimenResponse()
        {
            PrepareForSpecimenResponse request = new PrepareForSpecimenResponse();

            //Assert.IsAssignableFrom(typeof(EAC_U07), request.EAC_U07);

            Assert.That(request.MessageDateTime.ToString("yyyyMMddHHmmss"), Does.Match(""));

            Assert.AreEqual("EAR", request.MessageCode);

            Assert.AreEqual("U08", request.TriggerEvent);

            Guid g = new Guid();
            Assert.IsTrue(Guid.TryParse(request.ControlID, out g));

            Assert.IsFalse(g.ToString() == "00000000-0000-0000-0000-000000000000");

            string msg = request.Encode();

            Debug.Print(msg);

        }

        [Test]
        public void ParsePrepareForSpecimenResponse()
        {
            HL7Parser p = new HL7Parser();

            ParserResult pResult = null;

            string m = @"MSH|^~\&|||||20190322144038||EAR^U08^EAR_U08|89900867-5efc-4393-8dc2-c11baa88683f||2.5.1|||||||||PrepareForSpecimenResponse
            EQU||20190322144144
            ECD||LO^Prepare^ASTM|Y
            SAC|||123
            ECR|OK^Command completed successfully^HL70387";

            Assert.DoesNotThrow(() => { pResult = p.Parse(m); });

            Assert.That(pResult.MessageAccepted);

            EquipmentCommandResponse request = pResult.ParsedMessage as EquipmentCommandResponse;

            Assert.IsFalse(request.IsAcknowledge);

            Assert.IsNotNull(request);

            Assert.AreEqual(nameof(GeneralAcknowledgment), request.ExpectedAckID);

            Assert.AreEqual(string.Empty, request.ExpectedResponseID);

            Assert.AreEqual(request.GetValue("MSH-9-1"), "EAR");

            Assert.AreEqual(request.GetValue("MSH-9-2"), "U08");

            Assert.AreEqual(nameof(PrepareForSpecimenResponse), request.GetValue("MSH-21-1"));

            Assert.AreEqual("89900867-5efc-4393-8dc2-c11baa88683f", request.GetValue("MSH-10"));

            Assert.AreEqual(request.GetValue("COMMAND/ECD-2-1"), "LO");

            Assert.That(pResult.IsAcknowledge.HasValue && !pResult.IsAcknowledge.Value);

            GeneralAcknowledgment ack = pResult.Acknowledge as GeneralAcknowledgment;

            Assert.IsNotNull(ack);

            Assert.That(string.IsNullOrEmpty(ack.ExpectedResponseID));

            Assert.That(string.IsNullOrEmpty(ack.ExpectedAckID));

            Assert.AreEqual(ack.GetValue("MSA-1"), "AA");

            Assert.AreEqual(ack.GetValue("MSA-2"), "89900867-5efc-4393-8dc2-c11baa88683f");

            Assert.AreEqual(ack.GetValue("MSH-9-1"), "ACK");

            Assert.AreEqual(ack.GetValue("MSH-9-2"), "U08");

            Assert.AreEqual(nameof(GeneralAcknowledgment), ack.GetValue("MSH-21-1"));
        }
    }
}

