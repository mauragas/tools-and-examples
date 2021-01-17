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
  public class HostFileParser : IHostFileParser
  {
    private readonly ILogger log;

    public HostFileParser(ILogger logger) => this.log = logger;

    public HostFile Parse(string content)
    {
      var hostFile = new HostFile();
      using (var reader = new StringReader(RemoveComments(content)))
      {
        string line;
        while ((line = reader.ReadLine()) is not null)
        {
          var ipAndHostNames = new List<string>(CollapseWhitespaces(line).Split(' '));

          if (ipAndHostNames.Count < 2 ||
            !IPAddress.TryParse(ipAndHostNames[0], out var ipAddressInFile))
            continue;

          ipAndHostNames.GetRange(1, ipAndHostNames.Count - 1)
            .Where(hostName => IsValidHostName(hostName))
            .ToList()
            .ForEach(hostName =>
              hostFile.Hosts.Add(new Host
              {
                Ip = ipAddressInFile.ToString(),
                Name = hostName
              }));
        }
      }
      return hostFile;
    }

    public HostFile Parse(string content, string ipAddress)
    {
      if (!string.IsNullOrWhiteSpace(ipAddress) && !IsValidIpv4Address(ipAddress))
      {
        this.log.Error($"Failed to parse IP address {ipAddress}");
        Environment.Exit(0);
      }
      return new HostFile
      {
        Hosts = GetHosts(content, ipAddress)
      };
    }

    private static SortedSet<Host> GetHosts(string content, string ipAddress)
    {
      var hosts = new SortedSet<Host>();
      SplitToList(CollapseWhitespaces(RemoveComments(content)))
        .Where(hostName => IsValidHostName(hostName))
        .ToList()
        .ForEach(hostName =>
          hosts.Add(new Host
          {
            Ip = ipAddress,
            Name = hostName
          }));
      return hosts;
    }

    private static bool IsValidIpv4Address(string ip) =>
      Uri.CheckHostName(ip) == UriHostNameType.IPv4;

    private static string RemoveComments(string content) =>
      Regex.Replace(content, @"(?:#).*", string.Empty);

    private static string CollapseWhitespaces(string line) =>
      Regex.Replace(line, @"\s+", " ").Trim();

    private static bool IsValidHostName(string host) =>
      Uri.CheckHostName(host) == UriHostNameType.Dns;

    private static List<string> SplitToList(string content) =>
      CollapseWhitespaces(content).Split(' ').ToList();
  }
}
