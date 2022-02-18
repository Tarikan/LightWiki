using LightWiki.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LightWiki.Data.Configurations;

public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
{
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        builder.Property(e => e.CountryCode).HasMaxLength(3);
        builder.Property(e => e.Bio).HasMaxLength(256);
        builder.Property(e => e.Location).HasMaxLength(32);
        builder.Property(e => e.ContactEmail).HasMaxLength(254);

        builder.HasOne(e => e.User)
            .WithOne(u => u.UserInfo)
            .HasForeignKey<UserInfo>(e => e.UserId);
    }
}