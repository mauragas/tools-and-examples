using AirportList.Controllers;
using NUnit.Framework;

namespace AirportList.Tests.Controllers
{
    [TestFixture]
    public class AirportControllerTest
    {
        [Test]
        public void Index()
        {
            // Arrange
            var controller = new AirportController();

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void Distance()
        {
            // Arrange
            var controller = new AirportController();

            // Act
            var result = controller.Distance();

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
