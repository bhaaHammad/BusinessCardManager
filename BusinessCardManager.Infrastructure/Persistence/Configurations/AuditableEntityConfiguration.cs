using BusinessCardManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessCardManager.Infrastructure.Persistence.Configurations
{
    public static class AuditableEntityConfiguration
    {
        public static void ApplyAuditProperties(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(AuditableEntity.CreatedAt))
                        .HasDefaultValueSql("NOW()");

                    modelBuilder.Entity(entityType.ClrType)
                        .Property(nameof(AuditableEntity.UpdatedAt))
                        .IsRequired(false);
                }
            }
        }
    }
}
