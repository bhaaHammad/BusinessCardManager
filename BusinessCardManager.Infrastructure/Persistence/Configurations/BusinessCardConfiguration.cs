using BusinessCardManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessCardManager.Infrastructure.Persistence.Configurations
{
    public class BusinessCardConfiguration : IEntityTypeConfiguration<BusinessCard>
    {
        public void Configure(EntityTypeBuilder<BusinessCard> builder)
        {
            builder.ToTable("BusinessCards");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(b => b.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(b => b.Gender)
                   .HasMaxLength(10);

            builder.Property(b => b.DateOfBirth)
                   .HasColumnType("date");

            builder.Property(b => b.Email)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(b => b.Phone)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(b => b.Photo)
                    .HasColumnType("text");

            builder.Property(b => b.Address)
                   .HasMaxLength(500);

            builder.HasIndex(b => b.Email)
                   .IsUnique();

            builder.HasIndex(b => b.Phone)
                   .IsUnique();
        }
    }
}
