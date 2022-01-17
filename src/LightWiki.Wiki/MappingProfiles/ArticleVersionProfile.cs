using AutoMapper;
using LightWiki.Domain.Models;
using LightWiki.Features.ArticleVersions.Responses.Models;

namespace LightWiki.MappingProfiles;

public class ArticleVersionProfile : Profile
{
    public ArticleVersionProfile()
    {
        CreateMap<ArticleVersion, ArticleVersionModel>()
            .ForMember(dest => dest.CreationDate, opts => opts.MapFrom(src => src.CreatedAt));
    }
}