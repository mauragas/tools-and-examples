using AdAwayHost.Shared.Models;

namespace AdAwayHost
{
  public interface IHostFileParser
  {
    /// <summary>
    /// Parse given host file content and keep existing IP address.
    /// </summary>
    HostFile Parse(string content);

    /// <summary>
    /// Parse given host file content and assign new IP address.
    /// </summary>
    HostFile Parse(string content, string ipAddress);
  }
}
