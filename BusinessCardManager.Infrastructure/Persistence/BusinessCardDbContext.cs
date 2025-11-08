using BusinessCardManager.Domain.Entities;
using BusinessCardManager.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace BusinessCardManager.Infrastructure.Persistence
{
    public class BusinessCardDbContext : DbContext
    {
        public BusinessCardDbContext(DbContextOptions<BusinessCardDbContext> options) 
            : base(options) { }
        public DbSet<BusinessCard> BusinessCards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BusinessCardDbContext).Assembly);

            AuditableEntityConfiguration.ApplyAuditProperties(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }
    }
}
