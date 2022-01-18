using LightWiki.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LightWiki.Data.Configurations;

public class WorkspaceGroupAccessRuleConfiguration : IEntityTypeConfiguration<WorkspaceGroupAccessRule>
{
    public void Configure(EntityTypeBuilder<WorkspaceGroupAccessRule> builder)
    {
        builder.HasIndex(e => new { e.GroupId, e.WorkspaceId })
            .IsUnique();
    }
}