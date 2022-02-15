using LightWiki.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LightWiki.Data.Configurations;

public class ArticleAccessConfiguration : IEntityTypeConfiguration<ArticleAccess>
{
    public void Configure(EntityTypeBuilder<ArticleAccess> builder)
    {
        builder.HasIndex(e => new { e.PartyId, e.ArticleId })
            .IsUnique();
    }
}