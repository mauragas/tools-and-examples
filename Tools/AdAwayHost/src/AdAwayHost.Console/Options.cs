using System.Collections.Generic;
using CommandLine;

namespace AdAwayHost.Console
{
    public class Options
    {
        [Option('l', nameof(LogFilePath), HelpText = "Local workstation path to log file.", Required = false)]
        public string LogFilePath { get; set; }

        [Option('i', nameof(IpAddress), HelpText = "Host IP address used for downloaded hosts.", Required = false)]
        public string IpAddress { get; internal set; }

        [Option('f', nameof(HostsFilePath), HelpText = "Path to hosts file on local workstation.", Required = false)]
        public string HostsFilePath { get; set; }

        [Option('u', nameof(HostsSourceUrls), HelpText = "URL list of hosts source file.", Required = false)]
        public IEnumerable<string> HostsSourceUrls { get; set; }
    }
}