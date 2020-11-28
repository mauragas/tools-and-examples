using System.Linq;
using System;
using System.IO;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using AdAwayHost.Shared.Wrappers;

namespace AdAwayHost.Console
{
  public class AppConfiguration
  {
    public Options ConfigurationOptions { get; set; }
    private readonly ILogger log;
    private readonly IFileWrapper fileWrapper;

    public AppConfiguration(ILogger logger, IFileWrapper fileWrapper, string[] commandLineArguments)
    {
      this.log = logger;
      this.fileWrapper = fileWrapper;
      SetValuesFromConfigurationFile();
      if (commandLineArguments.Length > 0)
        CombineOptions(ParseCommandArguments(commandLineArguments));

      ConfigurationOptions.HostsFilePath = string.IsNullOrWhiteSpace(ConfigurationOptions.HostsFilePath) ?
          GetHostsFileLocation() :
          ConfigurationOptions.HostsFilePath.Trim();

      this.log.Information("Configuration values:\n{@Options}", ConfigurationOptions);
    }

    private void SetValuesFromConfigurationFile()
    {
      var builder = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile(GetAppSettinsFilePath(), optional: true, reloadOnChange: true);

      var configuration = builder.Build();
      ConfigurationOptions = new Options
      {
        HostsFilePath = configuration.GetSection(nameof(ConfigurationOptions.HostsFilePath))?.Value,
        LogFilePath = configuration.GetSection(nameof(ConfigurationOptions.LogFilePath))?.Value,
        IpAddress = configuration.GetSection(nameof(ConfigurationOptions.IpAddress))?.Value,
        HostsSourceUrls = configuration.GetSection(nameof(ConfigurationOptions.HostsSourceUrls))?.Get<List<string>>()
      };
    }

    /// <summary>
    /// While debugging returns AdAwayHost.Console/bin/Debug/netcoreapp3.1/appsettings.json
    /// After installation /usr/share/AdAwayHosts/appsettings.json
    /// </summary>
    private static string GetAppSettinsFilePath() =>
      Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "appsettings.json");

    private Options ParseCommandArguments(string[] commandLineArguments)
    {
      Options parsedArguments = null;
      _ = Parser.Default.ParseArguments<Options>(commandLineArguments)
        .WithParsed(options => parsedArguments = options)
        .WithNotParsed(errors => HandleParseError(errors));

      if (parsedArguments is null)
      {
        this.log.Error("Failed to parse command arguments.");
        Environment.Exit(0);
      }

      return parsedArguments;
    }

    /// <summary>
    /// In case of errors or --help or --version
    /// </summary>
    private void HandleParseError(IEnumerable<Error> errors)
    {
      if (!errors.Any(e => e is HelpRequestedError || e is VersionRequestedError))
        this.log.Error("Failed to parse command line arguments {ErrorTags}", errors.Select(e => e.Tag));
      Environment.Exit(0);
    }

    private void CombineOptions(Options parsedArguments)
    {
      if (!string.IsNullOrWhiteSpace(parsedArguments.LogFilePath))
        ConfigurationOptions.LogFilePath = parsedArguments.LogFilePath;

      if (!string.IsNullOrWhiteSpace(parsedArguments.IpAddress))
        ConfigurationOptions.IpAddress = parsedArguments.IpAddress;

      if (!string.IsNullOrWhiteSpace(parsedArguments.HostsFilePath))
        ConfigurationOptions.HostsFilePath = parsedArguments.HostsFilePath;

      parsedArguments.HostsSourceUrls = parsedArguments.HostsSourceUrls
          .Where(h => !string.IsNullOrWhiteSpace(h))
          .Select(h => h);
      if (parsedArguments.HostsSourceUrls.Any())
        ConfigurationOptions.HostsSourceUrls = parsedArguments.HostsSourceUrls;
    }

    private string GetHostsFileLocation()
    {
      var hostFileLocation = string.Empty;
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        hostFileLocation = "/etc/hosts";
      else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        hostFileLocation = @"C:\Windows\System32\drivers\etc\hosts";
      else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        hostFileLocation = "/private/etc/hosts";

      if (!this.fileWrapper.Exists(hostFileLocation))
        throw new FileNotFoundException($"Could not determine host file location for operating system {RuntimeInformation.OSDescription}");

      return hostFileLocation;
    }
  }
}
