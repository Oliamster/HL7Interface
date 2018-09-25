
using NHapi.Model.V251.Message;
using NHapiPlus.V251.Message;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
        }
    }
}
