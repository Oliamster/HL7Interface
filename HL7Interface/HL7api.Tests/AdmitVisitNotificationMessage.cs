using System;
using System.Diagnostics;
using NHapi.Model.V251.Message;
using HL7api.V251.Message;
using NUnit.Framework;


namespace HL7api.Tests
{
    [TestFixture]
    public class AdmitVisitNotificationMessage
    {
        [Test]
        public void CreateNewAdmitVisitNotification()
        {
            AdmitVisitNotification avn = new AdmitVisitNotification();

            Assert.IsAssignableFrom(typeof(ADT_A01), avn.ADT_A01);

            Assert.That(avn.MessageDateTime.ToString("yyyyMMddHHmmss"), Does.Match(""));

            Assert.AreEqual("ADT", avn.Code);

            Assert.AreEqual("A01", avn.Trigger);

            Guid g = new Guid();
            Assert.IsTrue(Guid.TryParse(avn.ControlID, out g));

            Assert.IsFalse(g.ToString() == "00000000-0000-0000-0000-000000000000");

            string msg = avn.Encode();

            Debug.Print(msg);
        }

        [Test]
        public void ParseAdmitVisitNotification()
        {
            AdmitVisitNotification avn = new AdmitVisitNotification();

            Assert.IsAssignableFrom(typeof(ADT_A01), avn.ADT_A01);

            string msg = avn.Encode();

            Debug.Print(msg);
        }
    }
}

