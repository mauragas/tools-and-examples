using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AdAwayHost.Shared.Wrappers;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using System.Linq;
using System.Runtime.InteropServices;

namespace AdAwayHost.Console
{
    class Program
    {
        private static AppConfiguration _appConfiguration;
        private static ILogger _log;
        private static HttpClient _httpClient;
        private static FileWrapper _fileWrapper;
        private static HostFileParser _hostFileParser;
        private static HostFileWriter _hostFileWriter;
        private static HostFileDownloader _hostFileDownloader;

        public static async Task Main(string[] args)
        {
            try
            {
                Setup(args);

#if !DEBUG
                System.Console.WriteLine("Press 'y' to continue.");
                if (System.Console.ReadKey().Key != ConsoleKey.Y)
                    return;
                System.Console.WriteLine();
#endif

                await ExecuteAsync();
            }
            catch (Exception e)
            {
                _log?.Fatal("Application failed: {Message}", e.Message);
                _log?.Verbose(e, "Application failed with unhandled exception.");
            }
        }

        private static void Setup(string[] args)
        {
            _log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            _fileWrapper = new FileWrapper();
            _appConfiguration = new AppConfiguration(_log, _fileWrapper, args);
            ConfigureLogger();
            _httpClient = new HttpClient();
            _hostFileParser = new HostFileParser(_log);
            _hostFileWriter = new HostFileWriter(_log, _fileWrapper, _hostFileParser, _appConfiguration.ConfigurationOptions.HostsFilePath);
            _hostFileDownloader = new HostFileDownloader(_log, _httpClient, _hostFileParser);
        }

        private static void ConfigureLogger()
        {
            var loggerConfiguration = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                loggerConfiguration.WriteTo.LocalSyslog(appName: nameof(AdAwayHost));

            if (!string.IsNullOrWhiteSpace(_appConfiguration.ConfigurationOptions.LogFilePath))
                loggerConfiguration.WriteTo.File(new CompactJsonFormatter(),
                    _appConfiguration.ConfigurationOptions.LogFilePath);

            _log = loggerConfiguration.CreateLogger();
        }

        private static async Task ExecuteAsync()
        {
            var remoteHostFileUrls = GetRemoteHostFileUrls();
            var adAwayHost = new AdAwayHost(_log, _hostFileDownloader, _hostFileWriter);
            await adAwayHost.UpdateHosts(remoteHostFileUrls, _appConfiguration.ConfigurationOptions.IpAddress);
        }

        private static List<Uri> GetRemoteHostFileUrls()
        {
            return _appConfiguration.ConfigurationOptions.HostsSourceUrls
                .Select(u => new Uri(u)).ToList();
        }
    }
}