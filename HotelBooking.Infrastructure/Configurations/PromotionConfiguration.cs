using HotelBooking.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Configurations;

public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Code).HasMaxLength(50).IsRequired();
        builder.Property(p => p.DiscountType).IsRequired();
        builder.Property(p => p.DiscountValue).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.MinBookingAmount).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.ExpiryDate).HasColumnType("datetime2").IsRequired();
        builder.Property(p => p.IsActive).HasDefaultValue(true);
        builder.Property(p => p.UsageLimit).IsRequired();
        builder.Property(p => p.UsageCount).HasDefaultValue(0);

        builder.HasIndex(p => p.Code).IsUnique();
    }
}
