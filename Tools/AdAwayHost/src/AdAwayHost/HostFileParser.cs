using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using AdAwayHost.Shared.Models;
using Serilog;

namespace AdAwayHost
{
    public class HostFileParser
    {
        private readonly ILogger _log;

        public HostFileParser(ILogger logger)
        {
            _log = logger;
        }

        public HostFile Parse(string content, string newIpAddress = "")
        {
            if (!string.IsNullOrWhiteSpace(newIpAddress) && !IPAddress.TryParse(newIpAddress, out _))
            {
                _log.Error($"Failed to parse IP address {newIpAddress}");
                Environment.Exit(0);
            }

            var hostFile = new HostFile();
            using (var reader = new StringReader(content))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = CollapseWhitespaces(line);

                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;

                    var ipAndHostNames = new List<string>(line.Split(" "));
                    if (ipAndHostNames.Count < 2 ||
                        ipAndHostNames[1].StartsWith("#") ||
                        !IPAddress.TryParse(ipAndHostNames[0], out var ipAddressInFile))
                        continue;

                    var ipAddress = string.IsNullOrWhiteSpace(newIpAddress) ? ipAddressInFile.ToString() : newIpAddress;

                    ipAndHostNames.GetRange(1, ipAndHostNames.Count - 1)
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

        private static string CollapseWhitespaces(string line)
        {
            line = Regex.Replace(line, @"\s+", " ").Trim(); // Collapse whitespaces
            return line;
        }
    }
}