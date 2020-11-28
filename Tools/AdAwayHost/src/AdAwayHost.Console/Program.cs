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
  internal class Program
  {
    private static AppConfiguration appConfiguration;
    private static ILogger log;
    private static HttpClient httpClient;
    private static FileWrapper fileWrapper;
    private static HostFileParser hostFileParser;
    private static HostFileWriter hostFileWriter;
    private static HostFileDownloader hostFileDownloader;

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
        log?.Fatal("Application failed: {Message}", e.Message);
        log?.Verbose(e, "Application failed with unhandled exception.");
      }
    }

    private static void Setup(string[] args)
    {
      log = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();

      fileWrapper = new FileWrapper();
      appConfiguration = new AppConfiguration(log, fileWrapper, args);
      ConfigureLogger();
      httpClient = new HttpClient();
      hostFileParser = new HostFileParser(log);
      hostFileWriter = new HostFileWriter(log, fileWrapper, hostFileParser, appConfiguration.ConfigurationOptions.HostsFilePath);
      hostFileDownloader = new HostFileDownloader(log, httpClient, hostFileParser);
    }

    private static void ConfigureLogger()
    {
      var loggerConfiguration = new LoggerConfiguration()
        .Enrich.WithExceptionDetails()
        .WriteTo.Console();

      if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        _ = loggerConfiguration.WriteTo.LocalSyslog(appName: nameof(AdAwayHost));

      if (!string.IsNullOrWhiteSpace(appConfiguration.ConfigurationOptions.LogFilePath))
        _ = loggerConfiguration.WriteTo.File(new CompactJsonFormatter(),
            appConfiguration.ConfigurationOptions.LogFilePath);

      log = loggerConfiguration.CreateLogger();
    }

    private static async Task ExecuteAsync()
    {
      var remoteHostFileUrls = GetRemoteHostFileUrls();
      var adAwayHost = new AdAwayHost(log, hostFileDownloader, hostFileWriter);
      await adAwayHost.UpdateHosts(remoteHostFileUrls, appConfiguration.ConfigurationOptions.IpAddress);
    }

    private static List<Uri> GetRemoteHostFileUrls() =>
      appConfiguration.ConfigurationOptions.HostsSourceUrls
        .Select(u => new Uri(u)).ToList();
  }
}
