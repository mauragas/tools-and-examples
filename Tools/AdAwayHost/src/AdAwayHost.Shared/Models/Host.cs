using System;

namespace AdAwayHost.Shared.Models
{
    public class Host : IComparable<Host>
    {
        public string Ip { get; set; }
        public string Name { get; set; }

        public int CompareTo(Host host)
        {
            var namesCompare = string.Compare(Name, host.Name);
            if (namesCompare == 0)
                return string.Compare(Ip, host.Ip);
            return namesCompare;
        }
    }
}