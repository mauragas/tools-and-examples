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
    private Mock<ILogger> loggerMock;
    private HostFileParser hostFileParser;
    private HostFileWriter hostFileWriter;
    private Mock<IFileWrapper> fileWrapperMock;
    private string pathToHostsFile;

    [OneTimeSetUp]
    public void Setup()
    {
      this.loggerMock = new Mock<ILogger>();
      this.fileWrapperMock = new Mock<IFileWrapper>();
      this.pathToHostsFile = "/etc/hosts";

      this.hostFileParser = new HostFileParser(this.loggerMock.Object);
      this.hostFileWriter = new HostFileWriter(this.loggerMock.Object,
        this.fileWrapperMock.Object,
        this.hostFileParser,
        this.pathToHostsFile);
    }

    [Test]
    public async Task Test_UpdateLocalHostFileAsync_FilesCombinedCorrectly()
    {
      // Arrange
      var inputHostFileContent = await TestHelpers.GetEmbeddedFileAsync("Resources/input-hosts");
      var oldHostFileContent = await TestHelpers.GetEmbeddedFileAsync("Resources/old-hosts");
      var combinedHostFileContentExpectedOutput = await TestHelpers.GetEmbeddedFileAsync("Resources/combined-hosts");

      _ = this.fileWrapperMock.Setup(f => f.Exists(this.pathToHostsFile))
        .Returns(() => true);

      _ = this.fileWrapperMock.Setup(f => f.ReadAllTextAsync(this.pathToHostsFile))
        .ReturnsAsync(() => oldHostFileContent);

      var combinedHostFileContentOutput = string.Empty;
      _ = this.fileWrapperMock.Setup(f => f.WriteAllTextAsync(this.pathToHostsFile, It.IsAny<string>()))
        .Callback((string pathToFile, string fileContent) => combinedHostFileContentOutput = fileContent);

      // Act
      var inputHostFileContentParsed = this.hostFileParser.Parse(inputHostFileContent);
      await this.hostFileWriter.UpdateLocalHostFileAsync(new List<HostFile> { inputHostFileContentParsed });

      // Assert
      Assert.That(inputHostFileContentParsed.Hosts.Count, Is.EqualTo(9));
      Assert.That(combinedHostFileContentOutput, Is.EqualTo(combinedHostFileContentExpectedOutput));
    }
  }
}
