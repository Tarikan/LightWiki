using LightWiki.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LightWiki.Data.Configurations;

public class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.HasIndex(e => e.Name)
            .IsUnique();

        builder.HasIndex(e => e.Slug)
            .IsUnique();

        builder.HasMany(e => e.Articles)
            .WithOne(a => a.Workspace)
            .HasForeignKey(a => a.WorkspaceId);

        builder.HasOne(e => e.RootArticle)
            .WithOne(a => a.RootedWorkspace)
            .HasForeignKey<Workspace>(w => w.RootArticleId);
    }
}