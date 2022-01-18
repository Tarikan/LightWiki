using LightWiki.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LightWiki.Data.Configurations;

public class WorkspacePersonalAccessRuleConfiguration : IEntityTypeConfiguration<WorkspacePersonalAccessRule>
{
    public void Configure(EntityTypeBuilder<WorkspacePersonalAccessRule> builder)
    {
        builder.HasIndex(e => new { e.UserId, e.WorkspaceId })
            .IsUnique();
    }
}