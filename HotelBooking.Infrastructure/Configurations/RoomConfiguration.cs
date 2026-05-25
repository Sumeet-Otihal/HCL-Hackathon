using HotelBooking.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.RoomNumber).HasMaxLength(50).IsRequired();
        builder.Property(r => r.FloorNumber).IsRequired();
        builder.Property(r => r.IsActive).HasDefaultValue(true);
        builder.Property(r => r.DeletedAt).HasColumnType("datetime2");

        builder.HasOne(r => r.RoomCategory)
            .WithMany(rc => rc.Rooms)
            .HasForeignKey(r => r.RoomCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Soft delete global query filter
        builder.HasQueryFilter(r => r.IsActive);
    }
}
