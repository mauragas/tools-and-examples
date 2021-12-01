using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AdAwayHost.Shared.Models
{
  public class HostFile
  {
    public static HostFile Empty => new();

    public SortedSet<Host> Hosts { get; set; }

    public HostFile() => Hosts = new SortedSet<Host>();

    public override string ToString()
    {
      var stringBuilder = new StringBuilder();
      Hosts.ToList().ForEach(h => stringBuilder.AppendLine($"{h.Ip} {h.Name}"));
      return stringBuilder.ToString();
    }
  }
}
