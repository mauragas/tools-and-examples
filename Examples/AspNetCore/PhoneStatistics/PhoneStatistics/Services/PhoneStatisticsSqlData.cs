using System.Collections.Generic;
using PhoneStatistics.Data;
using PhoneStatistics.Models;

namespace PhoneStatistics.Services
{
    public class PhoneStatisticsSqlData: IPhoneStatisticsData
    {
        private readonly PhoneStatisticsDbContext _repositoryDbContext;

        public PhoneStatisticsSqlData(PhoneStatisticsDbContext context)
        {
            _repositoryDbContext = context;
        }

        public IEnumerable<PhoneEvent> GetAll()
        {
            return _repositoryDbContext.PhoneEvents;
        }
    }
}