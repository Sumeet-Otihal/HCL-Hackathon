using HotelBooking.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Configurations;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Name).HasMaxLength(256).IsRequired();
        builder.Property(h => h.Description).HasMaxLength(2000);
        builder.Property(h => h.Address).HasMaxLength(500);
        builder.Property(h => h.City).HasMaxLength(256).IsRequired();
        builder.Property(h => h.Country).HasMaxLength(256).IsRequired();
        builder.Property(h => h.StarRating).IsRequired();
        builder.Property(h => h.Amenities).HasColumnType("nvarchar(max)");
        builder.Property(h => h.ImageUrls).HasColumnType("nvarchar(max)");
        builder.Property(h => h.IsActive).HasDefaultValue(true);
        builder.Property(h => h.CreatedAt).HasColumnType("datetime2");
        builder.Property(h => h.UpdatedAt).HasColumnType("datetime2");
        builder.Property(h => h.DeletedAt).HasColumnType("datetime2");

        // Soft delete global query filter
        builder.HasQueryFilter(h => h.IsActive);
    }
}
