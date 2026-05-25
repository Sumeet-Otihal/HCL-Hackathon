using HotelBooking.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.RazorpayOrderId).HasMaxLength(256);
        builder.Property(p => p.RazorpayPaymentId).HasMaxLength(256);
        builder.Property(p => p.RazorpaySignature).HasMaxLength(500);
        builder.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.Currency).HasMaxLength(10).IsRequired();
        builder.Property(p => p.Status).IsRequired();
        builder.Property(p => p.IsMock).IsRequired();
        builder.Property(p => p.CreatedAt).HasColumnType("datetime2");

        // One-to-one with Booking
        builder.HasOne(p => p.Booking)
            .WithOne(b => b.Payment)
            .HasForeignKey<Payment>(p => p.BookingId);
    }
}
