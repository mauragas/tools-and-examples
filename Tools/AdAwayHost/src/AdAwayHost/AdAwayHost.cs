using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;

namespace AdAwayHost
{
    public class AdAwayHost
    {
        private readonly ILogger _log;
        private readonly HostFileDownloader _hostFileDownloader;
        private readonly HostFileWriter _hostFileWriter;

        public AdAwayHost(ILogger logger, HostFileDownloader hostFileDownloader, HostFileWriter hostFileWriter)
        {
            _log = logger;
            _hostFileDownloader = hostFileDownloader;
            _hostFileWriter = hostFileWriter;
        }

        public async Task UpdateHosts(List<Uri> remoteHostFileUrls, string ipAddress)
        {
            _log.Information("Starting update with {Count} remote source file(s).", remoteHostFileUrls.Count);
            var hostFiles = await _hostFileDownloader.DownloadHostFilesAsync(remoteHostFileUrls, ipAddress);
            await _hostFileWriter.UpdateLocalHostFileAsync(hostFiles);
        }
    }
}