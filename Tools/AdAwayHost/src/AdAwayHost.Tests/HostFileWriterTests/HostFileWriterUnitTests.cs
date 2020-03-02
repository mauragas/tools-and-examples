using System.Collections.Generic;
using System.Threading.Tasks;
using AdAwayHost.Shared.Models;
using AdAwayHost.Shared.Wrappers;
using Moq;
using NUnit.Framework;
using Serilog;

namespace AdAwayHost.Tests.HostFileWriterTests
{
    public class HostFileWriterUnitTests
    {
        private Mock<ILogger> _loggerMock;
        private HostFileParser _hostFileParser;
        private HostFileWriter _hostFileWriter;
        private Mock<IFileWrapper> _fileWrapperMock;
        private string _pathToHostsFile;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _loggerMock = new Mock<ILogger>();
            _fileWrapperMock = new Mock<IFileWrapper>();
            _pathToHostsFile = "/etc/hosts";
            var oldHostFileContent = await TestHelpers.GetEmbeddedFileAsync("Resources/old-hosts");

            _hostFileParser = new HostFileParser(_loggerMock.Object);
            _hostFileWriter = new HostFileWriter(_loggerMock.Object, _fileWrapperMock.Object, _hostFileParser, _pathToHostsFile);
        }

        [Test]
        public async Task Test_UpdateLocalHostFileAsync_FilesCombinedCorrectly()
        {
            // Arrange
            var inputHostFileContent = await TestHelpers.GetEmbeddedFileAsync("Resources/input-hosts");
            var oldHostFileContent = await TestHelpers.GetEmbeddedFileAsync("Resources/old-hosts");
            var combinedHostFileContentExpectedOutput = await TestHelpers.GetEmbeddedFileAsync("Resources/combined-hosts");

            _fileWrapperMock.Setup(f => f.Exists(_pathToHostsFile))
                            .Returns(() => true);

            _fileWrapperMock.Setup(f => f.ReadAllTextAsync(_pathToHostsFile))
                            .ReturnsAsync(() => oldHostFileContent);

            var combinedHostFileContentOutput = string.Empty;
            _fileWrapperMock.Setup(f => f.WriteAllTextAsync(_pathToHostsFile, It.IsAny<string>()))
                            .Callback((string pathToFile, string fileContent) => combinedHostFileContentOutput = fileContent);

            // Act
            var inputHostFileContentParsed = _hostFileParser.Parse(inputHostFileContent);
            await _hostFileWriter.UpdateLocalHostFileAsync(new List<HostFile> { inputHostFileContentParsed });

            // Assert
            Assert.That(inputHostFileContentParsed.Hosts.Count, Is.EqualTo(9));
            Assert.That(combinedHostFileContentOutput, Is.EqualTo(combinedHostFileContentExpectedOutput));
        }
    }
}