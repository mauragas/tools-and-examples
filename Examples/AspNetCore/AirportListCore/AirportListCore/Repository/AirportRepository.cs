using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AirportListCore.Models;
using Newtonsoft.Json;

namespace AirportListCore.Repository
{
    public class AirportRepository
    {
        private string AirportSourceUrl { get; }
        public AirportRepository(string sourceUrl)
        {
            AirportSourceUrl = sourceUrl;
        }

        /// <summary>
        /// Download Json airport data as plain text string
        /// </summary>
        /// <returns>Airport information Json file</returns>
        private async Task<string> GetAirportJsonStream()
        {
            var client = new HttpClient();
            return await client.GetStringAsync(AirportSourceUrl);
        }

        /// <summary>
        /// Gets airport info in structure manner
        /// </summary>
        /// <returns>List of airports</returns>
        public async Task<List<Airport>> GetAirportInfoList()
        {
            var airportJsonStream = await GetAirportJsonStream();
            return JsonConvert.DeserializeObject<List<Airport>>(airportJsonStream);
        }
    }
}