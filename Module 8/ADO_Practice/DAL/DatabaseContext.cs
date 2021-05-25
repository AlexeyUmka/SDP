using DAL.Models;
using Microsoft.EntityFrameworkCore;

 

namespace DAL.Contexts
{
    public class DatabaseContext : DbContext
    {
        private readonly string _connectionString;

 

        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString;
        }

 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }

 

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("delivery");

 

            builder.Entity<Customer>()
                .ToTable(nameof(Customer))
                .HasKey(o => o.Id);
        }
    }
}