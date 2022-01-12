using AutoMapper;
using LightWiki.Infrastructure.Models;

namespace LightWiki.Infrastructure.AutoMapper;

public sealed class CollectionResultConverter<TSource, TDest> : ITypeConverter<CollectionResult<TSource>, CollectionResult<TDest>>
{
    public CollectionResult<TDest> Convert(
        CollectionResult<TSource> source,
        CollectionResult<TDest> destination,
        ResolutionContext context)
    {
        return new CollectionResult<TDest>(context.Mapper.Map<TDest[]>(source.Collection), source.Total);
    }
}