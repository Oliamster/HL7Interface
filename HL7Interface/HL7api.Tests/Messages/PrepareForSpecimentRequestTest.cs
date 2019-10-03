using HL7api.Parser;
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
            //ARANGE & ACK
            PrepareForSpecimenRequest prepare = new PrepareForSpecimenRequest("123");

            //ASSERTS
            //Assert.IsTrue(prepare.SampleID == "123");

            Assert.IsTrue(prepare.MessageID == "PrepareForSpecimenRequest");

            Assert.IsTrue(prepare.HL7Version == "2.5.1");

            Assert.IsTrue(prepare.ExpectedAckID == "GeneralAcknowledgment");

            Assert.IsTrue(prepare.ExpectedResponseID == "PrepareForSpecimenResponse");

            Assert.IsTrue(prepare.Trigger == "U07");

            Assert.IsTrue(prepare.Code == "EAC");

            Assert.IsTrue(prepare.ExpectedResponseType == "EAR_U08");

            Assert.IsTrue(prepare.ExpectedAckType == "ACK_U07");

            Assert.IsFalse(prepare.IsAcknowledge);

            Assert.IsTrue(Regex.Match(prepare.ControlID, pattern, RegexOptions.IgnoreCase).Success);

            string prepareRcv = @"MSH|^~\&|||||20190322123829||EAC^U07^EAC_U07|728d2456-339c-4d34-b7ad-c41b1e4cff34||2.5.1|||||||||PrepareForSpecimenRequest
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

            //Assert.IsTrue(prepareForSpecimen.SampleID == "123");

            Assert.IsTrue(prepareForSpecimen.MessageID == "PrepareForSpecimenRequest");


            Assert.IsTrue(prepareForSpecimen.MessageID == typeof(PrepareForSpecimenRequest).Name);

            Assert.IsTrue(prepareForSpecimen.HL7Version == "2.5.1");

            //Assert.IsTrue(prepare.ProcessingID == "P");

            Assert.IsTrue(prepareForSpecimen.ExpectedAckID == "GeneralAcknowledgment");

            Assert.IsTrue(prepareForSpecimen.ExpectedResponseID == "PrepareForSpecimenResponse");

            Assert.IsFalse(prepareForSpecimen.IsAcknowledge);

            Assert.AreEqual("728d2456-339c-4d34-b7ad-c41b1e4cff34", prepareForSpecimen.ControlID);

            Assert.AreEqual(prepareForSpecimen.MessageDateTime.ToString("yyyyMMddHHmmss"), "20190322123829");


            //ARANGE & ACT
            GeneralAcknowledgment ack = result.Acknowledge as GeneralAcknowledgment;

            //ASSERTS
            Assert.IsNotNull(ack);

            //Assert.IsTrue(ack.AckType == Constants.AckTypes.AA);

            Assert.IsTrue(ack.MessageID == "GeneralAcknowledgment");

            Assert.IsTrue(ack.Trigger == "U07");

            Assert.IsTrue(ack.Code == "ACK");

            Assert.IsTrue(prepare.HL7Version == "2.5.1");

            Assert.IsTrue(ack.ExpectedAckID == "");

            Assert.IsTrue(ack.ExpectedResponseID == "");

            Assert.IsTrue(ack.ExpectedResponseType == "");

            Assert.IsTrue(ack.ExpectedAckType == "");

            Assert.IsTrue(ack.IsAcknowledge);

            var regex1 = new Regex(@"(?im)^[{(]?[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?$");

            //StringAssert.Matches(ack.MessageID, regex1, "The MSH-10 shoul be a guid");

            Assert.IsTrue(ack.IsAckForRequest(prepareForSpecimen));
        }
    }
}
