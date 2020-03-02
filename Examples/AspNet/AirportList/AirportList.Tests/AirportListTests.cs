using AirportList.Repositories;
using NUnit.Framework;

namespace AirportList.Tests
{
    [TestFixture]
    public class AirportListTests
    {

        [Test]
        public void TestJsonResource()
        {
            var repository = new AirportRepository
            {
                AirportSourceUrl = "https://raw.githubusercontent.com/jbrooksuk/JSON-Airports/master/airports.json"
            };
            var airports = repository.GetAirportInfoList().Result;
            Assert.IsTrue(airports.Count > 1000);
        }
    }
}
