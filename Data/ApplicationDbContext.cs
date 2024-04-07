using CurrencyMonitoringWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyMonitoringWebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CurrencyExchange> CurrencyExchanges { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
