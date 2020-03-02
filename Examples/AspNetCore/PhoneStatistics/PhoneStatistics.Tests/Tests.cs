using System.Linq;
using NUnit.Framework;
using PhoneStatistics.Controllers;
using PhoneStatistics.Tests.MockDatabaseData;

namespace PhoneStatistics.Tests
{
    public class Tests
    {
        [Test]
        public void PhoneControllerTest()
        {
            var phoneController = new PhoneController(new MockDatabase());

            var result = phoneController.PhoneStatisticsToViewModel("01/01/2019", "01/26/2019");
            
            Assert.AreEqual(46, result.CountOfCall);
            Assert.AreEqual(14, result.CountOfSms);
            Assert.AreEqual(22222, result.CallEvents.Max(e=>e.Duration));
        }
    }
}