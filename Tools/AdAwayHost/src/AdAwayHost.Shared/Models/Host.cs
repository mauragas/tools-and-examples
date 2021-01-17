using System;

namespace AdAwayHost.Shared.Models
{
  public class Host : IComparable<Host>
  {
    public string Ip { get; set; }
    public string Name { get; set; }
    public int CompareTo(Host host) => string.Compare(Name, host.Name);
  }
}
