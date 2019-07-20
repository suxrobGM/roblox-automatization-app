using Microsoft.EntityFrameworkCore;
using RobloxAutomatization.Models;

namespace RobloxAutomatization.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<RobloxUser> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=app_db.sqlite");
            }
        }
    }
}
