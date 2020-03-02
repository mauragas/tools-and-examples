using System.Collections.Generic;
using PhoneStatistics.Models;

namespace PhoneStatistics.Services
{
    public interface IPhoneStatisticsData
    {
        IEnumerable<PhoneEvent> GetAll();
    }
}