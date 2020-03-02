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

        private readonly ILogger _log;
        private IFileWrapper _fileWrapper;
        private readonly HostFileParser _hostFileParser;

        public HostFileWriter(ILogger logger, IFileWrapper fileWrapper, HostFileParser hostFileParser, string hostsFilePath)
        {
            _log = logger;
            _fileWrapper = fileWrapper;
            _hostFileParser = hostFileParser;
            HostsFilePath = hostsFilePath;
        }

        public async Task UpdateLocalHostFileAsync(List<HostFile> hostFiles)
        {
            var localHostsFile = await GetLocalHostFileAsync().ConfigureAwait(false);

            hostFiles.Add(localHostsFile);
            var combinedHostFiles = await CombineHostFilesAsync(hostFiles);

            await UpdateLocalHostFileAsync(combinedHostFiles);
            _log.Information("Local hosts file is updated. Hosts count is {Count}", combinedHostFiles.Hosts.Count);
        }

        /// <summary>
        /// Combines and sort several hosts files content into one by removing duplicates.
        /// Use one line for each host entry.
        /// </summary>
        public async Task<HostFile> CombineHostFilesAsync(List<HostFile> hostFiles)
        {
            return await Task.Run(() =>
            {
                var resultHostFile = new HostFile();

                hostFiles.ForEach(hostFile =>
                    hostFile.Hosts.ToList().ForEach(host =>
                        resultHostFile.Hosts.Add(host))
                );

                return resultHostFile;
            });
        }

        /// <summary>
        /// Get file content from local host file
        /// </summary>
        private async Task<HostFile> GetLocalHostFileAsync()
        {
            if (!_fileWrapper.Exists(HostsFilePath))
            {
                _log.Information("Creating new hosts file {PathToHostsFile}.", HostsFilePath);
                _fileWrapper.Create(HostsFilePath);
                return new HostFile();
            }

            var hostFileContent = await _fileWrapper.ReadAllTextAsync(HostsFilePath);
            _log.Information("Local host file size of {sizeInBytes} bytes read from {PathToHostsFile}",
                hostFileContent.GetSizeInBytes(), HostsFilePath);
            var hostFile = _hostFileParser.Parse(hostFileContent);
            _log.Information("Local file {count} hosts parsed.", hostFile.Hosts.Count);
            return hostFile;
        }

        private async Task UpdateLocalHostFileAsync(HostFile hostFile)
        {
            await _fileWrapper.WriteAllTextAsync(HostsFilePath, hostFile.ToString());
        }
    }
}