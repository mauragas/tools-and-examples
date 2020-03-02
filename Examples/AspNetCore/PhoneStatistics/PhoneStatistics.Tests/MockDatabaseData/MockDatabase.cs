using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PhoneStatistics.Models;
using PhoneStatistics.Services;

namespace PhoneStatistics.Tests.MockDatabaseData
{
    public class MockDatabase : IPhoneStatisticsData
    {
        public IEnumerable<PhoneEvent> GetAll()
        {
            var pathToMockDatabaseJsonFile= AppContext.BaseDirectory +"MockDatabaseData/MockDatabaseData.json";
            using (var reader = File.OpenText(pathToMockDatabaseJsonFile))
            {
              return  JsonConvert.DeserializeObject<List<PhoneEvent>>(reader.ReadToEnd());  
            }    
        }
    }
}