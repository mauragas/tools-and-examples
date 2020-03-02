using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using AirportList.Models;
using Newtonsoft.Json;

namespace AirportList.Repositories
{
    public class AirportRepository
    {
        public string AirportSourceUrl { get; set; } = ConfigurationManager.AppSettings["AirportSourceUrl"];

        /// <summary>
        /// Download Json airport data as plain text string
        /// </summary>
        /// <returns>Airport information Json file</returns>
        private async Task<string> GetAirportJsonStream()
        {
            var client = new HttpClient();
            var response = await client.GetAsync(AirportSourceUrl);
            var content = await response.Content.ReadAsStringAsync();
            return content;
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