using DAL.Models;
using Microsoft.EntityFrameworkCore;


namespace DAL.Contexts
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
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