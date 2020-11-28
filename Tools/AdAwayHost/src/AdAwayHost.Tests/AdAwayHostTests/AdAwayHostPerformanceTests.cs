using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using AdAwayHost.Shared.Wrappers;
using Moq;
using NUnit.Framework;
using Serilog;

namespace AdAwayHost.Tests.AdAwayHostTests
{
  public class AdAwayHostPerformanceTests
  {
    private Mock<ILogger> loggerMock;
    private Mock<IFileWrapper> fileWrapperMock;
    private HostFileParser hostFileParser;
    private HostFileWriter hostFileWriter;
    private HostFileDownloader hostFileDownloader;

    private string pathToHostsFile;

    [OneTimeSetUp]
    public void Setup()
    {
      this.loggerMock = new Mock<ILogger>();
      this.fileWrapperMock = new Mock<IFileWrapper>();
      this.pathToHostsFile = "/etc/hosts";
      this.hostFileParser = new HostFileParser(this.loggerMock.Object);
      this.hostFileDownloader = new HostFileDownloader(this.loggerMock.Object,
        new HttpClient(), this.hostFileParser);
      this.hostFileWriter = new HostFileWriter(this.loggerMock.Object,
        this.fileWrapperMock.Object, this.hostFileParser, this.pathToHostsFile);
    }

    [Test]
    public async Task Test_Execute_TestPerformance()
    {
      // Arrange
      var stopWatch = new Stopwatch();
      var ipAddress = "0.0.0.0";

      var oldHostFileContent = await TestHelpers.GetEmbeddedFileAsync("Resources/old-hosts");
      _ = this.fileWrapperMock.Setup(f => f.ReadAllTextAsync(this.pathToHostsFile))
          .ReturnsAsync(() => oldHostFileContent);

      var remoteHostFileUrls = new List<Uri>
      {
        new Uri ("https://www.adawayapk.net/downloads/hostfiles/official/1_hosts.txt"),
        new Uri ("https://www.adawayapk.net/downloads/hostfiles/official/2_ad_servers.txt"),
        new Uri ("https://www.adawayapk.net/downloads/hostfiles/official/3_yoyohost.txt")
      };

      var adAwayHost = new AdAwayHost(this.loggerMock.Object, this.hostFileDownloader, this.hostFileWriter);

      // Act
      stopWatch.Start();
      await adAwayHost.UpdateHosts(remoteHostFileUrls, ipAddress);
      stopWatch.Stop();

      // Assert
      Assert.That(stopWatch.Elapsed, Is.AtMost(TimeSpan.FromSeconds(10)));
    }
  }
}
