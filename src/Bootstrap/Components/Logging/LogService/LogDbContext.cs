using Bootstrap.Components.Logging.LogService.Models.Entities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Bootstrap.Components.Logging.LogService
{
    public class LogDbContext : DbContext
    {
        public DbSet<Log> Logs { get; set; }

        public LogDbContext([NotNull] DbContextOptions options) : base(options)
        {
        }
    }
}