using Microsoft.EntityFrameworkCore;

namespace Monitoring.Model
{
    public class WebAppDbContext : DbContext
    {
        public WebAppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Production> Produces { get; set; }
    }
}
