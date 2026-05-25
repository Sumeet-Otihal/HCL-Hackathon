using HotelBooking.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(u => u.FirstName).HasMaxLength(256).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(256).IsRequired();
        builder.Property(u => u.Role).IsRequired();
        builder.Property(u => u.HotelId).IsRequired(false);
        builder.Property(u => u.LoyaltyPoints).HasDefaultValue(0);
        builder.Property(u => u.IsActive).HasDefaultValue(true);
        builder.Property(u => u.CreatedAt).HasColumnType("datetime2");
    }
}
