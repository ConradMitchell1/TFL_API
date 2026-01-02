using Microsoft.EntityFrameworkCore;
using TFL_API.Models;

namespace TFL_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Station> Stations => Set<Station>();
    }
}
