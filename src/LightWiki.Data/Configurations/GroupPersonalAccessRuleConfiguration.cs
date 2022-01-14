using LightWiki.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LightWiki.Data.Configurations;

public class GroupPersonalAccessRuleConfiguration : IEntityTypeConfiguration<GroupPersonalAccessRule>
{
    public void Configure(EntityTypeBuilder<GroupPersonalAccessRule> builder)
    {
        builder.HasIndex(e => new { e.GroupId, e.UserId })
            .IsUnique();
    }
}