
using NHapi.Model.V251.Message;
using HL7api.V251.Message;
using NUnit.Framework;
using System;
using System.Diagnostics;
using HL7api.Parser;

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
            Assert.IsTrue(Guid.TryParse(request.ControlID, out g));

            Assert.IsFalse(g.ToString() == "00000000-0000-0000-0000-000000000000");

            string msg = request.Encode();

            Debug.Print(msg);

        }

        [Test]
        public void ParseEquipmentCommandRequest()
        {
            HL7Parser p = new HL7Parser();
            ParserResult pResult = null;

            string m = @"MSH|^~\&|||||20181004003016||EAC^U07|d241a178-b714-49ce-9177-52a572e2f419||2.5.1|||||||||EquipmentCommandRequest
                        ECD||CL^Prepare for specimen acquisition^ASTM|Y";

            Assert.DoesNotThrow(() => { pResult = p.Parse(m); });

            Assert.That(pResult.MessageAccepted);

            EquipmentCommandRequest request = pResult.ParsedMessage as EquipmentCommandRequest;

            Assert.IsFalse(request.IsAcknowledge);

            Assert.IsNotNull(request);

            Assert.That(request.ExpectedAckID.Equals("GeneralAcknowledgment"));

            Assert.That(request.ExpectedResponseID.Equals(typeof(EquipmentCommandResponse).Name));

            Assert.AreEqual(request.GetValue("MSH-9-1"), "EAC");

            Assert.AreEqual(request.GetValue("MSH-9-2"), "U07");

            Assert.AreEqual(request.GetValue("MSH-21-1"), "EquipmentCommandRequest");

            Assert.AreEqual(request.GetValue("MSH-10"), "d241a178-b714-49ce-9177-52a572e2f419");

            Assert.AreEqual(request.GetValue("COMMAND/ECD-2-1"), "CL");

            Assert.That(pResult.IsAcknowledge.HasValue && !pResult.IsAcknowledge.Value);

            GeneralAcknowledgment ack = pResult.Acknowledge as GeneralAcknowledgment;

            Assert.IsNotNull(ack);

            Assert.That(string.IsNullOrEmpty(ack.ExpectedResponseID));

            Assert.That(string.IsNullOrEmpty(ack.ExpectedAckID));

            Assert.AreEqual(ack.GetValue("MSA-1"), "AA");

            Assert.AreEqual(ack.GetValue("MSA-2"), "d241a178-b714-49ce-9177-52a572e2f419");

            Assert.AreEqual(ack.GetValue("MSH-9-1"), "ACK");

            Assert.AreEqual(ack.GetValue("MSH-9-2"), "U07");

            Assert.AreEqual(ack.GetValue("MSH-21-1"), "GeneralAcknowledgment");
        }
    }
}

