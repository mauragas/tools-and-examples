using System.Collections.Generic;
using AirportList.Models;

namespace AirportList.ViewModels
{
    public class DistanceViewModel
    {
        public List<Airport> Airport1List { get; set; }
        public List<Airport> Airport2List { get; set; }

        public Airport Airport1 { get; set; }
        public Airport Airport2 { get; set; }
    }
}