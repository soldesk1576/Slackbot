using Microsoft.EntityFrameworkCore;
using StatusNotifier.Models.Entities;

namespace StatusNotifier.Data
{
    public class StatusNotifierDbContext : DbContext
    {
        public StatusNotifierDbContext(DbContextOptions<StatusNotifierDbContext> options)
            : base(options)
        {
        }

        public DbSet<StatusLog> StatusLogs { get; set; }
    }
}
