using DbConfig.Models;
using Microsoft.EntityFrameworkCore;

namespace DbConfig.Providers
{
    public class EntityConfigurationContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<Settings>? Settings { get; set; }

        public EntityConfigurationContext(string connectionString) =>
            _connectionString = connectionString;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_connectionString?.Length > 0)
                optionsBuilder.UseSqlServer(_connectionString);
            else
                optionsBuilder.UseInMemoryDatabase("InMemoryDatabase");
        }
    }
}