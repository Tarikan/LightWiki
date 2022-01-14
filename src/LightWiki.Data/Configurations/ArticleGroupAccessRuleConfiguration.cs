using LightWiki.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LightWiki.Data.Configurations;

public class ArticleGroupAccessRuleConfiguration : IEntityTypeConfiguration<ArticleGroupAccessRule>
{
    public void Configure(EntityTypeBuilder<ArticleGroupAccessRule> builder)
    {
        builder.HasIndex(e => new { e.ArticleId, e.GroupId })
            .IsUnique();
    }
}