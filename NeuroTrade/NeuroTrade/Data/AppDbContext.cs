using Microsoft.EntityFrameworkCore;
using NeuroTrade.Models;

namespace NeuroTrade.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserStock> UserStocks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=NeuroTrade.db"); // SQLite database file
        }
    }
}
