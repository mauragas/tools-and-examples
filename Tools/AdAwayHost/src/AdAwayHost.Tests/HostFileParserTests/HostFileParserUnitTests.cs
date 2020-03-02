using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Serilog;

namespace AdAwayHost.Tests.HostFileParserTests
{
    public class HostFileParserUnitTests
    {
        private Mock<ILogger> _loggerMock;
        private HostFileParser _hostFileParser;

        [OneTimeSetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger>();
            _hostFileParser = new HostFileParser(_loggerMock.Object);
        }

        [Test]
        public async Task Test_Parse_ParsedCorrectlyAsync()
        {
            // Arrange
            var inputHostFileContent = await TestHelpers.GetEmbeddedFileAsync("Resources/input-hosts");
            var outputHostFileContent = await TestHelpers.GetEmbeddedFileAsync("Resources/input-hosts-parsed");

            // Act
            var parsedHostFile = _hostFileParser.Parse(inputHostFileContent);

            // Assert
            Assert.That(parsedHostFile.Hosts.Count, Is.EqualTo(9));
            Assert.That(parsedHostFile.ToString(), Is.EqualTo(outputHostFileContent));
        }
    }
}