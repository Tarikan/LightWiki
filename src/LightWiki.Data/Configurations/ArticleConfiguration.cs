using LightWiki.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LightWiki.Data.Configurations;

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.HasIndex(a => new { a.Name, a.WorkspaceId })
            .IsUnique();

        builder.HasIndex(a => new { a.Slug, a.WorkspaceId })
            .IsUnique();

        builder.HasOne(a => a.Workspace)
            .WithMany(w => w.Articles)
            .HasForeignKey(a => a.WorkspaceId);

        builder.HasOne(a => a.RootedWorkspace)
            .WithOne(w => w.RootArticle);
    }
}