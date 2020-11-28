using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using AdAwayHost.Shared.Models;
using Serilog;
using System.Linq;

namespace AdAwayHost
{
  public class HostFileParser
  {
    private readonly ILogger log;
    private readonly string validHostnameRegex =
      @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$";

    public HostFileParser(ILogger logger) => this.log = logger;

    public HostFile Parse(string content, string newIpAddress = "")
    {
      if (!string.IsNullOrWhiteSpace(newIpAddress) && !IPAddress.TryParse(newIpAddress, out _))
      {
        this.log.Error($"Failed to parse IP address {newIpAddress}");
        Environment.Exit(0);
      }

      var hostFile = new HostFile();
      using (var reader = new StringReader(content))
      {
        string line;
        while ((line = reader.ReadLine()) is not null)
        {
          var ipAndHostNames = new List<string>(CollapseWhitespaces(line).Split(" "));
          if (ipAndHostNames.Count < 2 ||
            !IPAddress.TryParse(ipAndHostNames[0], out var ipAddressInFile))
            continue;

          var ipAddress = string.IsNullOrWhiteSpace(newIpAddress) ? ipAddressInFile.ToString() : newIpAddress;

          ipAndHostNames.GetRange(1, ipAndHostNames.Count - 1)
            .Where(hostName => IsValidHost(hostName)).ToList()
            .ForEach(hostName =>
              hostFile.Hosts.Add(new Host
              {
                Ip = ipAddress,
                Name = hostName
              }));
        }
      }
      return hostFile;
    }

    private static string CollapseWhitespaces(string line) =>
      Regex.Replace(line, @"\s+", " ").Trim();

    private bool IsValidHost(string host) =>
      Regex.IsMatch(host, this.validHostnameRegex);
  }
}
