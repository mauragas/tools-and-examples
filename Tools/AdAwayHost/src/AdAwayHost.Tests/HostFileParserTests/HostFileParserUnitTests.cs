using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Serilog;

namespace AdAwayHost.Tests.HostFileParserTests
{
  public class HostFileParserUnitTests
  {
    private Mock<ILogger> loggerMock;
    private HostFileParser hostFileParser;

    [OneTimeSetUp]
    public void Setup()
    {
      this.loggerMock = new Mock<ILogger>();
      this.hostFileParser = new HostFileParser(this.loggerMock.Object);
    }

    /// <summary>
    /// Method should keep existing hosts which have valid IP address.
    /// Comments should be ignored.
    /// </summary>
    [Test]
    public async Task Test_Parse_ExistingIpAddressesShouldBeKeptAsync()
    {
      // Arrange
      var inputHostFileContent = await TestHelpers.GetEmbeddedFileAsync("Resources/input-hosts");
      var outputHostFileContent = await TestHelpers.GetEmbeddedFileAsync("Resources/input-hosts-parsed");

      // Act
      var parsedHostFile = this.hostFileParser.Parse(inputHostFileContent);

      // Assert
      Assert.That(parsedHostFile.Hosts.Count, Is.EqualTo(9));
      Assert.That(outputHostFileContent, Is.EqualTo(parsedHostFile.ToString()));
    }

    /// <summary>
    /// Method should parse all values from given file,
    /// even if IP address value does not exist in the file.
    /// Comments should be ignored.
    /// </summary>
    [Test]
    public async Task Test_Parse_ParsedNewIpAddressIsAssignedAsync()
    {
      // Arrange
      var inputHostFileContent = await TestHelpers.GetEmbeddedFileAsync("Resources/input-hosts");
      var outputHostFileContent = await TestHelpers.GetEmbeddedFileAsync("Resources/input-hosts-parsed-with-new-ip");

      // Act
      var parsedHostFile = this.hostFileParser.Parse(inputHostFileContent, "0.0.0.0");

      // Assert
      Assert.That(parsedHostFile.Hosts.Count, Is.EqualTo(14));
      Assert.That(parsedHostFile.ToString(), Is.EqualTo(outputHostFileContent));
    }
  }
}
