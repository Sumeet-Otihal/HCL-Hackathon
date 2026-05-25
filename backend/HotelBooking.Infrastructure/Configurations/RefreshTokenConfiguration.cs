using HotelBooking.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Infrastructure.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);
        builder.Property(rt => rt.UserId).HasColumnType("nvarchar(450)").IsRequired();
        builder.Property(rt => rt.Token).HasMaxLength(500).IsRequired();
        builder.Property(rt => rt.ExpiresAt).HasColumnType("datetime2").IsRequired();
        builder.Property(rt => rt.IsRevoked).HasDefaultValue(false);
        builder.Property(rt => rt.CreatedAt).HasColumnType("datetime2");

        builder.HasIndex(rt => rt.Token).IsUnique();

        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
