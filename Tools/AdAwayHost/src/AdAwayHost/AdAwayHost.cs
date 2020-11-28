using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;

namespace AdAwayHost
{
  public class AdAwayHost
  {
    private readonly ILogger log;
    private readonly HostFileDownloader hostFileDownloader;
    private readonly HostFileWriter hostFileWriter;

    public AdAwayHost(ILogger logger, HostFileDownloader hostFileDownloader, HostFileWriter hostFileWriter)
    {
      this.log = logger;
      this.hostFileDownloader = hostFileDownloader;
      this.hostFileWriter = hostFileWriter;
    }

    public async Task UpdateHosts(List<Uri> remoteHostFileUrls, string ipAddress)
    {
      this.log.Information("Starting update with {Count} remote source file(s).", remoteHostFileUrls.Count);
      var hostFiles = await this.hostFileDownloader.DownloadHostFilesAsync(remoteHostFileUrls, ipAddress);
      await this.hostFileWriter.UpdateLocalHostFileAsync(hostFiles);
    }
  }
}
