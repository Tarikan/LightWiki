using LightWiki.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LightWiki.Data.Configurations;

public class WorkspaceAccessConfiguration : IEntityTypeConfiguration<WorkspaceAccess>
{
    public void Configure(EntityTypeBuilder<WorkspaceAccess> builder)
    {
        builder.HasIndex(e => new { e.PartyId, e.WorkspaceId })
            .IsUnique();
    }
}