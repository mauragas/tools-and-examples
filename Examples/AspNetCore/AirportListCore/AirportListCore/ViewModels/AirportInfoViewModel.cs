using System.ComponentModel.DataAnnotations;

namespace AirportListCore.ViewModels
{
    public class AirportInfoViewModel
    {
        [Display(Name = "Location ID")] public string LocationIdentifier { get; set; } // Iata
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Iso { get; set; }
        public int Status { get; set; }
        public string Name { get; set; }
        public string Continent { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }

    }
}