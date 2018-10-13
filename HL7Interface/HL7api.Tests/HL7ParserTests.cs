
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
    public class HL7ParserTests
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
            Assert.IsTrue(Guid.TryParse(avn.MessageID, out g));

            Assert.IsFalse(g.ToString() == "00000000-0000-0000-0000-000000000000");

            string msg = avn.Encode();

            Debug.Print(msg);

        }
    }
}

