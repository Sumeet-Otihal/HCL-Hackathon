using HotelBooking.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Configurations;

public class LoyaltyPointConfiguration : IEntityTypeConfiguration<LoyaltyPoint>
{
    public void Configure(EntityTypeBuilder<LoyaltyPoint> builder)
    {
        builder.HasKey(lp => lp.Id);
        builder.Property(lp => lp.UserId).HasColumnType("nvarchar(450)").IsRequired();
        builder.Property(lp => lp.Points).IsRequired();
        builder.Property(lp => lp.Type).IsRequired();
        builder.Property(lp => lp.CreatedAt).HasColumnType("datetime2");

        builder.HasOne(lp => lp.User)
            .WithMany()
            .HasForeignKey(lp => lp.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(lp => lp.Booking)
            .WithMany(b => b.LoyaltyPoints)
            .HasForeignKey(lp => lp.BookingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
