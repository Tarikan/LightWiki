using LightWiki.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LightWiki.Data.Configurations;

public class ArticlePersonalAccessRuleConfiguration : IEntityTypeConfiguration<ArticlePersonalAccessRule>
{
    public void Configure(EntityTypeBuilder<ArticlePersonalAccessRule> builder)
    {
        builder.HasIndex(e => new { e.UserId, e.ArticleId })
            .IsUnique();
    }
}