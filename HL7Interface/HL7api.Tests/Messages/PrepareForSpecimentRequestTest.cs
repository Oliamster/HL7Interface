﻿using HL7api.Parser;
using NHapiTools.Base.Util;
using NUnit.Framework;
using System;
using System.Text.RegularExpressions;

namespace HL7api.V251.Message.Tests
{
    public class PrepareForSpecimentRequestTest
    {

        HL7Parser parser = new HL7Parser();
        Guid g;
        string pattern = @"(?im)^[{(]?[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?$";

   
        [Test]
        public void A_CreateAndParsePrepareForSpecimenAcquisition()
        {
            PrepareForSpecimenRequest prepare = new PrepareForSpecimenRequest();

            Assert.AreEqual(nameof(PrepareForSpecimenRequest), prepare.MessageID);

            Assert.AreEqual("2.5.1", prepare.HL7Version);

            Assert.AreEqual(nameof(GeneralAcknowledgment), prepare.ExpectedAckID);

            Assert.AreEqual(nameof(PrepareForSpecimenResponse), prepare.ExpectedResponseID);

            Assert.AreEqual("U07", prepare.TriggerEvent);

            Assert.AreEqual("EAC", prepare.MessageCode);

            Assert.AreEqual("EAR_U08", prepare.ExpectedResponseType);

            Assert.AreEqual("ACK_U07", prepare.ExpectedAckType);

            Assert.IsFalse(prepare.IsAcknowledge);

            Assert.IsTrue(Regex.Match(prepare.ControlID, pattern, RegexOptions.IgnoreCase).Success);

            string prepareRcv = @"MSH|^~\&|||||20190322123829||EAC^U07^EAC_U07|728d2456-339c-4d34-b7ad-c41b1e4cff34||2.5.1|||||||||PrepareForSpecimenRequest|
            EQU||20190322123829
            ECD||LO^Prepare^ASTM|Y
            SAC|||123";

            ParserResult result = parser.Parse(prepareRcv);

            Assert.IsNotNull(result);

            Assert.IsTrue(result.MessageAccepted);

            Assert.IsTrue(result.IsAcknowledge.HasValue && !result.IsAcknowledge.Value);

            Assert.IsNotNull(result.ParsedMessage);

            Assert.IsTrue(result.ParsedMessage is PrepareForSpecimenRequest);

            PrepareForSpecimenRequest prepareForSpecimen = (PrepareForSpecimenRequest)result.ParsedMessage;

            Assert.IsTrue(prepareForSpecimen.GetValue("/COMMAND(0)/SPECIMEN_CONTAINER/SAC-3") == "123");

            Assert.IsTrue(prepareForSpecimen.MessageID == "PrepareForSpecimenRequest");

            Assert.IsTrue(prepareForSpecimen.MessageID == typeof(PrepareForSpecimenRequest).Name);

            Assert.IsTrue(prepareForSpecimen.HL7Version == "2.5.1");

            //Assert.IsTrue(prepare.ProcessingID == "P");

            Assert.AreEqual(nameof(GeneralAcknowledgment), prepareForSpecimen.ExpectedAckID);

            Assert.AreEqual(nameof(PrepareForSpecimenResponse), prepareForSpecimen.ExpectedResponseID);

            Assert.IsFalse(prepareForSpecimen.IsAcknowledge);

            Assert.AreEqual("728d2456-339c-4d34-b7ad-c41b1e4cff34", prepareForSpecimen.ControlID);

            //Assert.AreEqual("20190322123829", prepareForSpecimen.MessageDateTime.ToString("yyyyMMddHHmmss"));

            //ARANGE & ACT
            GeneralAcknowledgment ack = result.Acknowledge as GeneralAcknowledgment;

            //ASSERTS
            Assert.IsNotNull(ack);

            //Assert.IsTrue(ack.AckType == AckTypes.AA);

            Assert.IsTrue(ack.MessageID == "GeneralAcknowledgment");

            Assert.IsTrue(ack.TriggerEvent == "U07");

            Assert.IsTrue(ack.MessageCode == "ACK");

            Assert.IsTrue(prepare.HL7Version == "2.5.1");

            Assert.IsTrue(ack.ExpectedAckID == "");

            Assert.AreEqual(string.Empty, ack.ExpectedAckID);

            Assert.AreEqual(string.Empty, ack.ExpectedResponseID);

            Assert.AreEqual(string.Empty, ack.ExpectedResponseType);

            Assert.AreEqual(string.Empty, ack.ExpectedAckType);

            Assert.IsTrue(ack.IsAcknowledge);

            var regex1 = new Regex(@"(?im)^[{(]?[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?$");

            //StringAssert.Matches(ack.MessageID, regex1, "The MSH-10 shoul be a guid");

            Assert.IsTrue(ack.IsAckForRequest(prepareForSpecimen));

        }

        [Test]
        public void ParseShouldNotFail()
        {
            string prepareRcv = @"MSH|^~\&|||||20190322123829||EAC^U07^EAC_U07|728d2456-339c-4d34-b7ad-c41b1e4cff34||2.5.1|||||||||PrepareForSpecimenRequest
            EQU||20190322123829
            ECD||LO^Prepare^ASTM|Y
            SAC|||123";

        }
    }
}
