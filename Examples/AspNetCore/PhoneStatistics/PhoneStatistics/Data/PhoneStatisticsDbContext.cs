using Microsoft.EntityFrameworkCore;
using PhoneStatistics.Models;

namespace PhoneStatistics.Data
{
    public class PhoneStatisticsDbContext : DbContext
    {
        public PhoneStatisticsDbContext(DbContextOptions options) : base(options)
        {      
        }
        public DbSet<PhoneEvent> PhoneEvents { get; set; }
    }
}