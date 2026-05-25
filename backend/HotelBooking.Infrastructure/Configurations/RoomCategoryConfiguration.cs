using HotelBooking.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Configurations;

public class RoomCategoryConfiguration : IEntityTypeConfiguration<RoomCategory>
{
    public void Configure(EntityTypeBuilder<RoomCategory> builder)
    {
        builder.HasKey(rc => rc.Id);
        builder.Property(rc => rc.Name).IsRequired();
        builder.Property(rc => rc.Description).HasMaxLength(1000);
        builder.Property(rc => rc.PricePerNight).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(rc => rc.MaxOccupancy).IsRequired();
        builder.Property(rc => rc.Amenities).HasColumnType("nvarchar(max)");
        builder.Property(rc => rc.ImageUrls).HasColumnType("nvarchar(max)");
        builder.Property(rc => rc.IsActive).HasDefaultValue(true);
        builder.Property(rc => rc.DeletedAt).HasColumnType("datetime2");

        builder.HasOne(rc => rc.Hotel)
            .WithMany(h => h.RoomCategories)
            .HasForeignKey(rc => rc.HotelId)
            .OnDelete(DeleteBehavior.Restrict);

        // Soft delete global query filter
        builder.HasQueryFilter(rc => rc.IsActive);
    }
}
