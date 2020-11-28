using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AdAwayHost.Shared.Models;
using System.Linq;
using Serilog;
using AdAwayHost.Shared;

namespace AdAwayHost
{
  public class HostFileDownloader
  {
    private readonly ILogger log;
    private readonly HttpClient httpClient;
    private readonly HostFileParser hostFileParser;

    public HostFileDownloader(ILogger logger, HttpClient httpClient, HostFileParser hostFileParser)
    {
      this.log = logger;
      this.httpClient = httpClient;
      this.hostFileParser = hostFileParser;
    }

    public async Task<List<HostFile>> DownloadHostFilesAsync(List<Uri> uri, string ipAddress)
    {
      var hostFileArray = await Task.WhenAll(uri.Select(u =>
        Task.Run(async () =>
          await DownloadHostFileAsync(u, ipAddress))).ToList());
      return hostFileArray.ToList();
    }

    private async Task<HostFile> DownloadHostFileAsync(Uri uri, string ipAddress)
    {
      var fileContent = await this.httpClient.GetStringAsync(uri);
      var hostFile = this.hostFileParser.Parse(fileContent, ipAddress);
      this.log.Information("Remote host file size of {size} bytes downloaded and {count} hosts parsed from {url}.",
        fileContent.GetSizeInBytes(), hostFile.Hosts.Count, uri.ToString());
      return hostFile;
    }
  }
}
