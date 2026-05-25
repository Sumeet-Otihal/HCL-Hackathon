using HotelBooking.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.UserId).HasColumnType("nvarchar(450)").IsRequired();
        builder.Property(b => b.CheckInDate).HasColumnType("datetime2").IsRequired();
        builder.Property(b => b.CheckOutDate).HasColumnType("datetime2").IsRequired();
        builder.Property(b => b.TotalNights).IsRequired();
        builder.Property(b => b.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(b => b.DiscountAmount).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
        builder.Property(b => b.FinalAmount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(b => b.Status).IsRequired();
        builder.Property(b => b.ReservationNumber).HasMaxLength(256).IsRequired();
        builder.Property(b => b.PaymentStatus).IsRequired();
        builder.Property(b => b.CreatedAt).HasColumnType("datetime2");

        builder.HasIndex(b => b.ReservationNumber).IsUnique();

        builder.HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Room)
            .WithMany(r => r.Bookings)
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
