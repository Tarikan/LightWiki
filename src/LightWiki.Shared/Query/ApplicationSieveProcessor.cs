using LightWiki.Domain.Models;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;

namespace LightWiki.Shared.Query;

public class ApplicationSieveProcessor : SieveProcessor
{
    public ApplicationSieveProcessor(
        IOptions<SieveOptions> options)
        : base(options)
    {
    }

    protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
    {
        mapper.Property<Article>(p => p.Name)
            .CanFilter()
            .CanSort();

        mapper.Property<Article>(p => p.UserId)
            .CanFilter();

        mapper.Property<Group>(p => p.Name)
            .CanSort()
            .CanFilter();

        mapper.Property<ArticleVersion>(p => p.CreatedAt)
            .CanFilter()
            .CanSort();

        mapper.Property<ArticleVersion>(p => p.UserId)
            .CanFilter();

        return mapper;
    }
}