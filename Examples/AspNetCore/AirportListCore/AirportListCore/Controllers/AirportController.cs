using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportListCore.Models;
using AirportListCore.Repository;
using AirportListCore.ViewModels;
using GeoCoordinatePortable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AirportListCore.Controllers
{
    public class AirportController : Controller
    {
        private AirportRepository Repository { get; } 

        public AirportController(IConfiguration configuration)
        {
            Repository = new AirportRepository(configuration["AirportSourceUrl"]);
        }
        
        /// <summary>
        /// Main page fo displaying list of airport data
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Index()
        {
            var data = await AirportInfoToViewModel();
            return View(data);
        }

        /// <summary>
        /// Display page for distance calculation between airports
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Distance()
        {
            var euOnlyAirports = await GetListOfEuAirports();

            var distanceViewModel = new DistanceViewModel
            {
                Airport1List = euOnlyAirports,
                Airport2List = euOnlyAirports
            };
            return View(distanceViewModel);
        }

        /// <summary>
        /// Calculate distance between airports by Iata
        /// </summary>
        /// <param name="airport1">Iata from first airport</param>
        /// <param name="airport2">Iata from second airport</param>
        /// <returns>Distance between airports</returns>
        [HttpPost]
        public async Task<string> GetDistance(string airport1, string airport2)
        {
            if (airport1 == airport2)
                return "0 km.";

            const string failedMessage = "Failed to calculate distance between airports.";

            try
            {
                var euOnlyAirports = await GetListOfEuAirports();

                var airport1Data = euOnlyAirports.FirstOrDefault(a => a.Iata.Contains(airport1));
                var airport2Data = euOnlyAirports.FirstOrDefault(a => a.Iata == airport2);

                if (airport1Data == null || airport2Data == null)
                    return "Failed to get coordination data, cannot calculate the distance.";

                var airport1Coordinate = new GeoCoordinate(airport1Data.Lat, airport1Data.Lon);
                var airport2Coordinate = new GeoCoordinate(airport2Data.Lat, airport2Data.Lon);

                var distanceInKm = airport1Coordinate.GetDistanceTo(airport2Coordinate) / 1000;

                return distanceInKm <= 0 ? failedMessage : $"{distanceInKm:F2} km.";
            }
            catch
            {
                return failedMessage;
            }
        }
        /// <summary>
        /// Converting airport model to view model which is used in user interface
        /// </summary>
        /// <returns>Airport data view model</returns>
        private async Task<List<AirportInfoViewModel>> AirportInfoToViewModel()
        {
            var euOnlyAirports = await GetListOfEuAirports();

            return euOnlyAirports.Select(euOnlyAirport => new AirportInfoViewModel
            {
                LocationIdentifier = euOnlyAirport.Iata,
                Longitude = euOnlyAirport.Lon,
                Latitude = euOnlyAirport.Lat,
                Iso = euOnlyAirport.Iso,
                Status = euOnlyAirport.Status,
                Name = euOnlyAirport.Name,
                Continent = euOnlyAirport.Continent,
                Type = euOnlyAirport.Type,
                Size = euOnlyAirport.Size,
            })
                .ToList();
        }

        private async Task<List<Airport>> GetListOfEuAirports()
        {
            return (await Repository.GetAirportInfoList()).Where(airport => airport.Continent.Contains("EU")).OrderBy(b => b.Iata).ToList();
        }
    }
}
