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
        private Mock<ILogger> _loggerMock;
        private Mock<IFileWrapper> _fileWrapperMock;
        private HostFileParser _hostFileParser;
        private HostFileWriter _hostFileWriter;
        private HostFileDownloader _hostFileDownloader;

        private string _pathToHostsFile;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _loggerMock = new Mock<ILogger>();
            _fileWrapperMock = new Mock<IFileWrapper>();
            _pathToHostsFile = "/etc/hosts";
            var oldHostFileContent = await TestHelpers.GetEmbeddedFileAsync("Resources/old-hosts");
            _hostFileParser = new HostFileParser(_loggerMock.Object);
            _hostFileDownloader = new HostFileDownloader(_loggerMock.Object, new HttpClient(), _hostFileParser);
            _hostFileWriter = new HostFileWriter(_loggerMock.Object, _fileWrapperMock.Object, _hostFileParser, _pathToHostsFile);
        }

        [Test]
        public async Task Test_Execute_TestPerformance()
        {
            // Arrange
            var stopWatch = new Stopwatch();
            var ipAddress = "0.0.0.0";

            var oldHostFileContent = await TestHelpers.GetEmbeddedFileAsync("Resources/old-hosts");
            _fileWrapperMock.Setup(f => f.ReadAllTextAsync(_pathToHostsFile))
                .ReturnsAsync(() => oldHostFileContent);

            var remoteHostFileUrls = new List<Uri> {
                new Uri ("https://adaway.org/hosts.txt"),
                new Uri ("https://hosts-file.net/ad_servers.txt"),
                new Uri ("https://pgl.yoyo.org/adservers/serverlist.php?hostformat=hosts&showintro=0&mimetype=plaintext")
            };

            var adAwayHost = new AdAwayHost(_loggerMock.Object, _hostFileDownloader, _hostFileWriter);

            // Act
            stopWatch.Start();
            await adAwayHost.UpdateHosts(remoteHostFileUrls, ipAddress);
            stopWatch.Stop();

            // Assert
            Assert.That(stopWatch.Elapsed, Is.AtMost(TimeSpan.FromSeconds(10)));
        }
    }
}