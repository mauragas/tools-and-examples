using System.Collections.Generic;
using System.Threading.Tasks;
using AdAwayHost.Shared.Models;
using AdAwayHost.Shared.Wrappers;
using System.Linq;
using Serilog;
using AdAwayHost.Shared;

namespace AdAwayHost
{
  public class HostFileWriter
  {
    public string HostsFilePath { get; private set; }

    private readonly ILogger log;
    private readonly IFileWrapper fileWrapper;
    private readonly HostFileParser hostFileParser;

    public HostFileWriter(ILogger logger, IFileWrapper fileWrapper, HostFileParser hostFileParser, string hostsFilePath)
    {
      this.log = logger;
      this.fileWrapper = fileWrapper;
      this.hostFileParser = hostFileParser;
      HostsFilePath = hostsFilePath;
    }

    public async Task UpdateLocalHostFileAsync(List<HostFile> hostFiles)
    {
      var localHostsFile = await GetLocalHostFileAsync().ConfigureAwait(false);
      hostFiles = hostFiles.Prepend(localHostsFile).ToList();
      var combinedHostFiles = await CombineHostFilesAsync(hostFiles);
      await UpdateLocalHostFileAsync(combinedHostFiles);
      this.log.Information("Local hosts file is updated. Hosts count is {Count}", combinedHostFiles.Hosts.Count);
    }

    /// <summary>
    /// Combines and sort several hosts files content into one by removing duplicates.
    /// Use one line for each host entry.
    /// </summary>
    public static async Task<HostFile> CombineHostFilesAsync(List<HostFile> hostFiles) =>
      await Task.Run(() =>
        {
          var resultHostFile = new HostFile();
          hostFiles.ForEach(hostFile =>
            hostFile.Hosts.ToList().ForEach(host =>
              resultHostFile.Hosts.Add(host))
          );
          return resultHostFile;
        });

    /// <summary>
    /// Get file content from local host file
    /// </summary>
    private async Task<HostFile> GetLocalHostFileAsync()
    {
      if (!this.fileWrapper.Exists(HostsFilePath))
      {
        this.log.Information("Creating new hosts file {PathToHostsFile}.", HostsFilePath);
        this.fileWrapper.Create(HostsFilePath);
        return new HostFile();
      }

      var hostFileContent = await this.fileWrapper.ReadAllTextAsync(HostsFilePath);
      this.log.Information("Local host file size of {sizeInBytes} bytes read from {PathToHostsFile}",
        hostFileContent.GetSizeInBytes(), HostsFilePath);
      var hostFile = this.hostFileParser.Parse(hostFileContent);
      this.log.Information("Local file {count} hosts parsed.", hostFile.Hosts.Count);
      return hostFile;
    }

    private async Task UpdateLocalHostFileAsync(HostFile hostFile) =>
      await this.fileWrapper.WriteAllTextAsync(HostsFilePath, hostFile.ToString());
  }
}
